using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources.DTO.Payment.Request
{
    public class CheckPaymentRequest
    {
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public int? AccountId { get; set; }
    }
}
