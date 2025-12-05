using ETC.EPAY.Integration.Backend.EpayRequest.Base;
using ETC.EPAY.Integration.Backend.Resources.Enums;
using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Backend.EpayRequest
{
    public class PayGwRefundRequest : PayGwDataRequestBase
    {
        public string ReferenceId { get; set; }
        public string OrderCode { get; set; }
        public RefundType RefundType { get; set; }
        public long Amount { get; set; }
        public long RefundTime { get; set; }

    }
}
