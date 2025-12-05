using ETC.EPAY.Integration.Backend.EpayRequest;
using ETC.EPAY.Integration.Request;
using ETC.EPAY.Integration.Response;
using ETC.EPAY.Integration.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Services.PaymentGateway
{
    public interface IPayGwService
    {
        Task<BaseResult<PayGwTokenDataResponse>> GetTokenAsync(CancellationToken cancellationToken);
        Task<BaseResult<PayGwOrderDataResponse>> CreateOrderAsync(PayGwCreateOrderDataRequest request, CancellationToken cancellationToken, string token);
        Task<BaseResult<PayGwOrderStatusDataResponse>> CheckOrderStatusAsync(PayGwCheckOrderStatusDataRequest request, CancellationToken cancellationToken, string token);
        Task<BaseResult<PayGwRefundResponse>> RefundAsync(PayGwRefundRequest request, CancellationToken cancellationToken, string token);
        Task<BaseResult<PayGwCheckRefundStatusResponse>> CheckRefundStatusAsync(PayGwCheckRefundStatusRequest request, CancellationToken cancellationToken, string token);
        Task<BaseResult<PayGwDeleteTokenResponse>> DeleteTokenAsync(PayGwDeleteTokenRequest request, CancellationToken cancellationToken, string token);
        Task<BaseResult<PayGwRetrieveTokenResponse>> RetrieveTokenAsync(PayGwRetrieveTokenRequest request, CancellationToken cancellationToken, string token);
        Task<BaseResult<PayGwQrCusPaymentResponse>> QrCusPaymentAsync(QrCusPaymentRequest request, CancellationToken cancellationToken, string token);
    
    
        Task<BaseResult<bool>> CreateOrderReturnUrlAsync(string epayResult, CancellationToken cancellationToken);
        Task<BaseResult<bool>> CreateOrderCallbackAsync(PayGwResponse<IpnPayGatewayRequest> request, CancellationToken cancellationToken);
        Task<BaseResult<bool>> RefundCallbackAsync(PayGwResponse<IpnPayGatewayRefundRequest> request, CancellationToken cancellationToken);
    }
}
