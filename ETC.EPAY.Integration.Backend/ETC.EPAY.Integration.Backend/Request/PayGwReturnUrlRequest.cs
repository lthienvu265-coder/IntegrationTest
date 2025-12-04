using ETC.EPAY.Integration.Resources.Enums;
using ETC.EPAY.Integration.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Request
{
    public class PayGwReturnUrlRequest : PayGwDataRequestBase
    {
        public string OrderCode { get; set; }
        public string TransCode { get; set; }
        public string PartnerCode { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public TransStatus TransStatus { get; set; }
    }
}
