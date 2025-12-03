using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Response
{
    public abstract class PayGwDataResponseBase
    {
        public string MerchantCode { get; set; }
        public string TransId { get; set; }
        public string MessageType { get; set; }
        public long TimeResponse { get; set; }
        public PayGwErrorCode ErrorCode { get; set; }
        public string ErrorDesc { get; set; }


        public bool IsSuccess => ErrorCode == PayGwErrorCode.Success;

        public bool IsFailed => ErrorCode != PayGwErrorCode.Success;
    }
}
