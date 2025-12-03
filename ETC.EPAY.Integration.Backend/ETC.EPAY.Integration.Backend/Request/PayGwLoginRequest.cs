using ETC.EPAY.Integration.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Request
{
    public class PayGwLoginRequest : PayGwDataRequestBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public static PayGwLoginRequest Create(string userName, string password, string merchantCode) =>
            new()
            {
                UserName = userName,
                Password = password,
                TimeRequest = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                MessageType = Constants.PayGwMessageType.GetToken,
                MerchantCode = merchantCode
            };
    }
}
