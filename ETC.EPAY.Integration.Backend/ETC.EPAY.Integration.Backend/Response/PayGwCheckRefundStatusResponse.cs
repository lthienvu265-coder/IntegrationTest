using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
        public PayGwPaymentType RefundType { get; set; }
        public PaymentStatus PaymentMethod { get; set; }
        public TransactionStatus TransStatus { get; set; }
        public int ErrorCode { get; set; }
    }
}
