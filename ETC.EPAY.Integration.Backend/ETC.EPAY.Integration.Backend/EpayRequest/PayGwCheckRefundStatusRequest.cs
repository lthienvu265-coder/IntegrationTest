using ETC.EPAY.Integration.Backend.EpayRequest.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Backend.EpayRequest
{
    public class PayGwCheckRefundStatusRequest : PayGwDataRequestBase
    {
        public string TransCode { get; set; }
        public string OrderCode { get; set; }
        public string RefundRequestId { get; set; }
        public long RefundTime { get; set; }
        public long TimeRequest { get; set; }
    }
}
