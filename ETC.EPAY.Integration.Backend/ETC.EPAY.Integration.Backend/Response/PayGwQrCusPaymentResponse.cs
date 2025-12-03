using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Response
{
    public class PayGwQrCusPaymentResponse : PayGwDataResponseBase
    {
        public string OrderCode { get; set; }
        public string TransCode { get; set; }
        public long TotalAmount { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
