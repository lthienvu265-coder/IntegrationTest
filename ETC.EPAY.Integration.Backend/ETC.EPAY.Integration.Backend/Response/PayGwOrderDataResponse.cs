using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Response
{
    public class PayGwOrderDataResponse : PayGwDataResponseBase
    {
        /// <summary>
        ///Url để chuyển qua trang thanh toán của EPAY
        /// </summary>
        public required string PaymentUrl { get; set; }
        public string? QrCode { get; set; }
        public int TimeLimit { get; set; }
        public int? QrExpire { get; set; }
        public string? DeepLink { get; set; }
    }
}
