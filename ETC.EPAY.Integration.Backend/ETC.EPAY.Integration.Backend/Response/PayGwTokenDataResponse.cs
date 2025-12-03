using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Response
{
    public class PayGwTokenDataResponse : PayGwDataResponseBase
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Token expire time in seconds
        /// </summary>
        public int ExpiresIn { get; set; }

        public bool IsSoonExpired()
        {

            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long expireTime = (TimeResponse + ExpiresIn * 1000) / 1000; // convert to seconds
            long diff = expireTime - currentTime; // calculate the difference
            return diff <= 10; // if the difference is less than 10 seconds, return true
        }
    }
}
