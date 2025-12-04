using ETC.EPAY.Integration.Extensions;
using ETC.EPAY.Integration.Resources.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ETC.EPAY.Integration.Request
{
    public class PayGwIpnPayGatewayRequest : PayGwDataRequestBase
    {
        public string OrderCode { get; set; }
        public string TransCode { get; set; }
        public PaymentType PaymentMethod { get; set; }
        public PayGwStatus TransStatus { get; set; }
        public string CardNumber { get; set; }
        public string PartnerCode { get; set; }
        public PayGwErrorCode ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
        public string InvoiceUrl { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
    }
}
