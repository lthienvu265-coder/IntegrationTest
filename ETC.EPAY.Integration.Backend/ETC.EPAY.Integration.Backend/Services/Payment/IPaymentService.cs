using ETC.EPAY.Integration.Backend.EpayRequest;
using ETC.EPAY.Integration.Resources.DTO.Payment.Request;
using ETC.EPAY.Integration.Resources.DTO.Payment.Response;
using ETC.EPAY.Integration.Response;
using ETC.EPAY.Integration.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Services.Payment
{
    public interface IPaymentService
    {
        /// <summary>
        /// Chức năng: đăng kí hồ sơ phía Isofh và khởi tạo giao dịch POS hoặc sinh mã QR
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseResult<PayGwOrderDataResponse>> GeneratePaymentAsync(PayGwCreateOrderDataRequest request, string token);

        /// <summary>
        /// Chức năng: kiểm tra trạng thái giao dịch
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseResult<CheckPaymentResponse>> CheckPaymentAsync(CheckPaymentRequest request);
    }
}
