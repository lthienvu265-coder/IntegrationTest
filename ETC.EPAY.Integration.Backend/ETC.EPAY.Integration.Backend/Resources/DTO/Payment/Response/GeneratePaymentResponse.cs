using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources.DTO.Payment.Response
{
    public class GeneratePaymentResponse
    {
        public TransStatus PartnerPaymentStatus { get; set; }
        public PaymentType Type { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string QrImage { get; set; }
        public DateTime? RequestDatetimeUtc { get; set; }
        public DateTime? ResponseDatetimeUtc { get; set; }
        public string Invoice { get; set; }
        public ChangePriceInformation ChangePriceInformation { get; set; }
        public string? PaymentUrl { get; set; }
        public int ExpiredSeconds { get; set; }
        public DateTime ExpiredTimeUtc { get; set; }
    }

    public sealed class ChangePriceInformation
    {
        #region Properties
        public bool IsChange { get; init; }
        public decimal OldPrice { get; init; }
        public decimal NewPrice { get; init; }
        #endregion

        #region Constructor
        public ChangePriceInformation(decimal oldPrice, decimal newPrice)
        {
            this.OldPrice = oldPrice;
            this.NewPrice = newPrice;

            this.IsChange = OldPrice == NewPrice ? false : true;
        }
        #endregion
    }
}
