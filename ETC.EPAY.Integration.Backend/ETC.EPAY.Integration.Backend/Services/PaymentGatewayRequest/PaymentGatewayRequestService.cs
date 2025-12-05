using ETC.EPAY.Integration.Backend.EpayRequest;
using ETC.EPAY.Integration.Backend.Request;

namespace ETC.EPAY.Integration.Backend.Services.PaymentGatewayRequest
{
    public class PaymentGatewayRequestService : IPaymentGatewayRequestService
    {
        public async Task<PayGwCheckOrderStatusDataRequest> CreatePayGwCheckOrderStatusDataRequest(CheckStatusRequest request)
        {
            var payGwCheckOrderStatusDataRequest = PayGwCheckOrderStatusDataRequest.Create(request.OrderCode, "IACV");
            return payGwCheckOrderStatusDataRequest;
        }

        public async Task<PayGwCreateOrderDataRequest> CreatePayGwCreateOrderDataRequest(CreatePaymentRequest request)
        {
            DateTime dt = DateTime.UtcNow;
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
            return payGwCreateOrderRequest;
        }
    }
}
