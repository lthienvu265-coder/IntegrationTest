using ETC.EPAY.Integration.Backend.Request;
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
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPayGwService _payGwService;
        private readonly IPaymentService _paymentService;
        public PaymentController(IPayGwService payGwService, IPaymentService paymentService)
        {
            _payGwService = payGwService;
            _paymentService = paymentService;
        }

        [HttpPost("create_payment")]
        public async Task<BaseResult<PayGwOrderDataResponse>> CreatePayment(PayGwCreatePaymentRequest request, CancellationToken cancellationToken)
        {
            DateTime dt = DateTime.UtcNow;
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;

            var payGwCreateOrderRequest = new PayGwCreateOrderDataRequest
            {
                MessageType = "create_order",
                TransId = Guid.NewGuid().ToString(),
                TimeRequest = new DateTimeOffset(dt).ToUnixTimeMilliseconds(),
                MerchantPassword = "123456",
                MerchantCode = "IACV",
                OrderCode = request.OrderCode,
                PaymentType = Integration.Resources.Enums.PaymentType.Now,
                BusinessType = "0000",
                TotalAmount = request.TotalAmount,
                OrderAmount = request.TotalAmount,
                FeeAmount = 0,
                OrderDescription = "Đơn hàng cho IACV",
                Currency = "VND",
                ReturnUrl = "https://paymenthub-form-dev.epayservices.com.vn/clear/S.02.01.60.91.2023822.1692687299554?session=17cdfd18-cf09-4b8c-adb7-64edec680c4d-1dc61de4-4be0-42ab-adab-098362df2637&partnerCode=VNEID_PARTNER",
                CancelUrl = "http://localhost:5678",
                AgainUrl = "http://localhost:5678",
                TimeLimit = 120,
                TotalGoods = 1,
                DetailGoods = new List<PayGwGoodDetail>
                {
                    new PayGwGoodDetail
                    {
                        GoodCode = Guid.NewGuid().ToString(),
                        GoodsPrice = request.TotalAmount,
                        GoodsQuantity = 1,
                        GoodsName = "Đơn hàng cho IACV"
                    }
                },
                ChannelCode = "02",
                PaymentMethod = request.PaymentMethod == 1 ? "04" : "00",
                DeviceId = request.DeviceId,
            };

            var paymentResult = await _paymentService.GeneratePaymentAsync(payGwCreateOrderRequest, token);
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

        [HttpPost("refund")]
        public async Task<BaseResult<PayGwRefundResponse>> Refund(PayGwRefundRequest request, CancellationToken cancellationToken)
        {
            var payGwTokenDataResponse = await _payGwService.GetTokenAsync(cancellationToken);
            var token = payGwTokenDataResponse.Data.Token;
            var payGwRefundResponse = await _payGwService.RefundAsync(request, cancellationToken, token);
            return payGwRefundResponse;
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
