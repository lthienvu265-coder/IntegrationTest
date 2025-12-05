using ETC.EPAY.Integration.Backend.EpayRequest;
using ETC.EPAY.Integration.Backend.Request;
using ETC.EPAY.Integration.Backend.Services.PaymentGatewayRequest;
using ETC.EPAY.Integration.Models;
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
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPayGwService _payGwService;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentGatewayRequestService _paymentGatewayRequestService;
        public PaymentController(IPayGwService payGwService, IPaymentService paymentService, IPaymentGatewayRequestService paymentGatewayRequestService)
        {
            _payGwService = payGwService;
            _paymentService = paymentService;
            _paymentGatewayRequestService = paymentGatewayRequestService;
        }

        [HttpPost("create_payment")]
        public async Task<BaseResult<PayGwOrderDataResponse>> CreatePayment(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var payGwCreateOrderRequest = await _paymentGatewayRequestService.CreatePayGwCreateOrderDataRequest(request);
            var paymentResult = await _paymentService.GeneratePaymentAsync(payGwCreateOrderRequest, token);
            return paymentResult;
        }

        [HttpPost("check_status")]
        public async Task<BaseResult<PayGwOrderStatusDataResponse>> CheckStatus(CheckStatusRequest request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var payGwCheckOrderStatusDataRequest = await _paymentGatewayRequestService.CreatePayGwCheckOrderStatusDataRequest(request);
            var payGwOrderStatusDataResponse = await _payGwService.CheckOrderStatusAsync(payGwCheckOrderStatusDataRequest, cancellationToken, token);
            return payGwOrderStatusDataResponse;
        }
    }
}
