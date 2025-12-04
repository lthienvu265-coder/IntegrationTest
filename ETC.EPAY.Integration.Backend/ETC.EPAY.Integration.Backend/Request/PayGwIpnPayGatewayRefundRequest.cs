using ETC.EPAY.Integration.Backend.Resources.Enums;
using ETC.EPAY.Integration.Resources.Enums;
using ETC.EPAY.Integration.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Request
{
    public class PayGwIpnPayGatewayRefundRequest : PayGwDataRequestBase
    {
        public string RefundRequestId { get; set; }
        public string OrderCode { get; set; }
        public string TransCode { get; set; }
        public string ReferenceId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public TransStatus TransStatus { get; set; }
        public RefundType RefundType { get; set; }
        public long Amount { get; set; }
        public long RefundTime { get; set; }
    }
}
