using ETC.EPAY.Integration.Backend.EpayRequest;
using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources.DTO.Payment.Request
{
    public sealed class GeneratePaymentRequest
    {
        [JsonPropertyName("c06VerifyStatus")]
        public bool? C06VerifyStatus { get; set; } = false;
        public string OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentType Type { get; set; }
        public KioskType kioskType { get; set; }

        public string KioskId { get; set; }
        public int? AccountId { get; set; }
        public string DeviceName { get; set; } = "KIOSK_SAINT_PAUL";
        public string? PosId { get; set; }
        public string? PosIp { get; set; }
        public PayGwCardInfo? CardInfo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerAddress { get; set; }
        public string DichVuId { get; set; }
        public string TenDichVu { get; set; }
        public string TypeCardAccount { get; set; }
        public string Token { get; set; }
        public string ChannelCode { get; set; }
    }
}
