using ETC.EPAY.Integration.Request;
using ETC.EPAY.Integration.Response;
using ETC.EPAY.Integration.Results;
using ETC.EPAY.Integration.Services.Payment;
using ETC.EPAY.Integration.Services.PaymentGateway;
using Microsoft.AspNetCore.Mvc;

namespace ETC.EPAY.Integration.Backend.Controllers
{
    [ApiController]
    [Route("api/ipn")]
    public class IpnController : ControllerBase
    {
        private readonly IPayGwService _payGwService;
        public IpnController(IPayGwService payGwService)
        {
            _payGwService = payGwService;
        }

        [HttpPost("epay/payment-result")]
        public async Task<BaseResult<bool>> CreateOrderCallback(PayGwResponse<IpnPayGatewayRequest> request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var createOrderCallbackResult = await _payGwService.CreateOrderCallbackAsync(request, cancellationToken);
            return createOrderCallbackResult;
        }

        [HttpPost("epay/refund-result")]
        public async Task<BaseResult<bool>> RefundCallback(PayGwResponse<IpnPayGatewayRefundRequest> request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var createOrderCallbackResult = await _payGwService.RefundCallbackAsync(request, cancellationToken);
            return createOrderCallbackResult;
        }
    }
}
