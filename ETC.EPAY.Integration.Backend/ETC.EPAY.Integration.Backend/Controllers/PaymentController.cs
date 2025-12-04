using ETC.EPAY.Integration.Models;
using ETC.EPAY.Integration.Request;
using ETC.EPAY.Integration.Resources.DTO.Payment.Request;
using ETC.EPAY.Integration.Resources.DTO.Payment.Response;
using ETC.EPAY.Integration.Response;
using ETC.EPAY.Integration.Results;
using ETC.EPAY.Integration.Services.Payment;
using ETC.EPAY.Integration.Services.PaymentGateway;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Threading;

namespace ETC.EPAY.Integration.Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPayGwService _payGwService;
        private readonly IPaymentService _paymentService;
        public PaymentController(ILogger<PaymentController> logger, IPayGwService payGwService, IPaymentService paymentService)
        {
            _logger = logger;
            _payGwService = payGwService;
            _paymentService = paymentService;
        }

        [HttpPost("generate_token")]
        public async Task<BaseResult<Guid>> GenerateTokenSocket(CancellationToken cancellationToken)
        {
            var tokenSocket = Guid.NewGuid().ToString();
            return new BaseResult<Guid>(tokenSocket);
        }

        [HttpPost("create_payment")]
        public async Task<BaseResult<PayGwOrderDataResponse>> CreatePayment(PayGwCreateOrderDataRequest request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var paymentResult = await _paymentService.GeneratePaymentAsync(request, token);
            return paymentResult;
        }

        [HttpPost("check_status")]
        public async Task<BaseResult<PayGwOrderStatusDataResponse>> CheckStatus(PayGwCheckOrderStatusDataRequest request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var payGwOrderStatusDataResponse = await _payGwService.CheckOrderStatusAsync(request, cancellationToken, token);
            return payGwOrderStatusDataResponse;
        }

        [HttpPost("get_ipn_transfer_result")]
        public async Task<BaseResult<bool>> CreateOrderCallback(PayGwResponse<PayGwIpnPayGatewayRequest> request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var createOrderCallbackResult = await _payGwService.CreateOrderCallbackAsync(request, cancellationToken);
            return createOrderCallbackResult;
        }

        [HttpPost("refund")]
        public async Task<BaseResult<PayGwRefundResponse>> Refund(PayGwRefundRequest request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var payGwRefundResponse = await _payGwService.RefundAsync(request, cancellationToken, token);
            return payGwRefundResponse;
        }

        [HttpPost("get_ipn_refund_result")]
        public async Task<BaseResult<bool>> RefundCallback(PayGwResponse<PayGwIpnPayGatewayRefundRequest> request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var createOrderCallbackResult = await _payGwService.RefundCallbackAsync(request, cancellationToken);
            return createOrderCallbackResult;
        }

        [HttpPost("check_refund_status")]
        public async Task<BaseResult<PayGwCheckRefundStatusResponse>> CheckRefundStatus(PayGwCheckRefundStatusRequest request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var payGwCheckRefundStatusResponse = await _payGwService.CheckRefundStatusAsync(request, cancellationToken, token);
            return payGwCheckRefundStatusResponse;
        }

        [HttpPost("delete_token")]
        public async Task<BaseResult<PayGwDeleteTokenResponse>> DeleteToken(PayGwDeleteTokenRequest request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var payGwDeleteTokenResponse = await _payGwService.DeleteTokenAsync(request, cancellationToken, token);
            return payGwDeleteTokenResponse;
        }

        [HttpPost("retrieve_token")]
        public async Task<BaseResult<PayGwRetrieveTokenResponse>> RetrieveToken(PayGwRetrieveTokenRequest request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var payGwRetrieveTokenResponse = await _payGwService.RetrieveTokenAsync(request, cancellationToken, token);
            return payGwRetrieveTokenResponse;
        }

    }
}
