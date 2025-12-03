using ETC.EPAY.Integration.Resources.DTO.Payment.Request;
using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.DataAccess
{
    public interface IPaymentLogDAO
    {
        Task<Models.PaymentLog?> GetByIdAsync(string id);
        /// <summary>
        /// Chức năng: xuất báo cáo giao dịch theo tháng
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<(bool hasValue, IEnumerable<Models.PaymentLog> data)> GetReportMonthlyAsync(DateOnly date);

        /// <summary>
        /// Chức năng: xuất báo cáo giao dịch theo ngày
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<(bool hasValue, IEnumerable<Models.PaymentLog> data)> GetReportDailyAsync(DateOnly date);

        /// <summary>
        /// Chức năng: tạo mới một payment-log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<(bool isSuccess, Models.PaymentLog data)> CreateAsync(Models.PaymentLog model);

        /// <summary>
        /// Chức năng: cập nhật payment-log
        /// </summary>
        /// <param name="model"></param>
        /// <param name="checkFinalStatus">Kiểm tra trạng thái cuối trước khi update</param>
        /// <returns></returns>
        Task<(bool isSuccess, Models.PaymentLog data)> UpdatePartnerStatusAsync(Models.PaymentLog model, bool checkFinalStatus = true);

        /// <summary>
        /// Chức năng: lấy giá trị payment-log dựa trên điều kiện request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<(bool isSuccess, Models.PaymentLog data)> CheckPaymentAsync(CheckPaymentRequest request);

        /// <summary>
        /// Chức năng: lấy giá trị payment-log dựa trên điều kiện request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<(bool isSuccess, Models.PaymentLog data)> CheckPaymentEpayAsync(CheckPaymentEpayRequest request);

        /// <summary>
        /// Chức năng: lấy một payment-log dựa vào partnerTransId
        /// </summary>
        /// <param name="partnerTransId"></param>
        /// <returns></returns>
        Task<(bool isSuccess, Models.PaymentLog data)> GetByPartnerIdAsync(string partnerTransId);

        /// <summary>
        /// Chức năng: Lấy thông tin thanh toán thành công để in lại phiếu thanh toán
        /// </summary>
        /// <param name="maHoSo"></param>
        /// <param name="maNb"></param>
        /// <param name="maPhieuThu"></param>
        /// <returns></returns>
        Task<(bool hasValue, Models.PaymentLog data)> GetForScanAgainAsync(string maHoSo, string maNb, string maPhieuThu);

        /// <summary>
        /// Chức năng: lấy ra một đơn hàng đã thanh toán thành công
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<(bool hasValue, Models.PaymentLog data)> GetPaidOrderAsync(string orderId);

        Task<IEnumerable<Models.PaymentLog>> GetUncompletedAsync(DateOnly inDate, params PaymentFlow[] flows);

        /// <summary>
        /// Chức năng: lấy ra một đơn hàng theo orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<(bool hasValue, Models.PaymentLog data)> GetByOrderAsync(string orderId);

        /// <summary>
        /// Chức năng: cập nhật thông tin retry
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> UpdateLastRetryAsync(Models.PaymentLog model);
    }
}
