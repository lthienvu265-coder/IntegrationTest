using ETC.EPAY.Integration.Backend.EpayRequest.Base;
using ETC.EPAY.Integration.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Backend.EpayRequest
{
    public class PayGwCheckOrderStatusDataRequest : PayGwDataRequestBase
    {
        public required string OrderCode { get; set; }
        public static PayGwCheckOrderStatusDataRequest Create(string orderCode, string merchantCode)
        {
            return new PayGwCheckOrderStatusDataRequest
            {
                OrderCode = orderCode,
                MerchantCode = merchantCode,
                TimeRequest = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                MessageType = Constants.PayGwMessageType.CheckStatus
            };
        }
    }
}
