using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources.Enums
{
    public enum PaymentType
    {
        QrMobileBanking = 1,
        QrEpay = 2,
        POS = 3,
        Cash = 4,
        CardReader = 5,
        CreditAndDebitCard = 6,
        DomesticCard = 7,
    }
}
