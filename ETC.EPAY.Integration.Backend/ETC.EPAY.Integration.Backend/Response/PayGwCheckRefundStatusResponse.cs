using ETC.EPAY.Integration.Backend.Resources.Enums;
using ETC.EPAY.Integration.Resources.Enums;

namespace ETC.EPAY.Integration.Response
{
    public class PayGwCheckRefundStatusResponse : PayGwDataResponseBase
    {
        public RefundInfo RefundInfo { get; set; }
        public long TimeResponse { get; set; }
        public string Checksum { get; set; }
    }
    public class RefundInfo
    {
        public string ReferenceId { get; set; }
        public string MerchantCode { get; set; }
        public string OrderCode { get; set; }
        public string TransCode { get; set; }
        public long RefundTime { get; set; }
        public long Amount { get; set; }
        public RefundType RefundType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public TransStatus TransStatus { get; set; }
        public int ErrorCode { get; set; }
    }
}
