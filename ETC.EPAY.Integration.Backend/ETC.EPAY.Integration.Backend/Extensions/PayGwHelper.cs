using ETC.EPAY.Integration.Models;
using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Extensions
{
    public static class PayGwHelper
    {
        public static string GetPaymentMethod(this PaymentLog paymentLog)
        {
            return paymentLog.payment_type switch
            {
                PaymentType.QrMobileBanking => "04",
                PaymentType.QrEpay => "01",
                PaymentType.POS => "00",
                PaymentType.CardReader => "02",
                PaymentType.CreditAndDebitCard => "03",
                PaymentType.DomesticCard => "02",
                PaymentType.Cash => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
