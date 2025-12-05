using ETC.EPAY.Integration.Backend.EpayRequest;
using ETC.EPAY.Integration.Backend.Request;

namespace ETC.EPAY.Integration.Backend.Services.PaymentGatewayRequest
{
    public interface IPaymentGatewayRequestService
    {
        Task<PayGwCreateOrderDataRequest> CreatePayGwCreateOrderDataRequest(CreatePaymentRequest request); 
        Task<PayGwCheckOrderStatusDataRequest> CreatePayGwCheckOrderStatusDataRequest(CheckStatusRequest request);
    }
}
