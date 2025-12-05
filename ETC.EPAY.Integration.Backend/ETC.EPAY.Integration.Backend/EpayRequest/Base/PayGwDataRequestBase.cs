using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Backend.EpayRequest.Base
{
    public abstract class PayGwDataRequestBase
    {
        public string MerchantCode { get; set; }
        public string TransId { get; set; }
        public long TimeRequest { get; set; }
        public string MessageType { get; set; }
    }
}
