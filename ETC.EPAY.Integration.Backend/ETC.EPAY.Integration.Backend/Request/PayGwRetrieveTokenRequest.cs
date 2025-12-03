using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Request
{
    public class PayGwRetrieveTokenRequest : PayGwDataRequestBase
    {
        public string CustomerCode { get; set; }
        public string OrderCode { get; set; }
        public string PaymentType { get; set; }
    }
}
