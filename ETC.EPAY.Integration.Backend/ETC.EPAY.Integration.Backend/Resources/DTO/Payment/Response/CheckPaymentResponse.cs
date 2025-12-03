using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources.DTO.Payment.Response
{
    public class CheckPaymentResponse
    {
        public PaymentStatus PartnerPaymentStatus { get; set; }
        public PaymentStatus IsofhPaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public DateTime? RequestDatetimeUtc { get; set; }
        public DateTime? ResponseDatetimeUtc { get; set; }
        public PaymentType Type { get; set; }
        public string Invoice { get; set; }
        public ChangePriceInformation ChangePriceInformation { get; set; }
    }
}
