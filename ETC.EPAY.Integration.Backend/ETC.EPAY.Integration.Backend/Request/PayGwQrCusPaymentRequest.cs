using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Request
{
    public class PayGwQrCusPaymentRequest : PayGwDataRequestBase
    {
        public string QrCode { get; set; }
        public string OrderCode { get; set; }
        public long PaymentAmount { get; set; }
    }
}
