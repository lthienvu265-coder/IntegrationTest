using ETC.EPAY.Integration.Models;
using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ETC.EPAY.Integration.Resources.Constants;

namespace ETC.EPAY.Integration.Response
{
    public class PayGwOrderStatusDataResponse : PayGwDataResponseBase
    {
        public List<PayGwTransactionInfo>? TransactionInfos { get; set; }


        public bool GetPaidTransaction(out PayGwTransactionInfo? transactionInfo)
        {
            transactionInfo = TransactionInfos?.FirstOrDefault(x => x.TransStatus == PayGwStatus.Paid);
            return transactionInfo != null;
        }

        public PayGwTransactionInfo? GetFinalTransactionInfo()
        {
            return TransactionInfos?.FirstOrDefault(x => x.TransStatus is PayGwStatus.Paid or PayGwStatus.Fail or PayGwStatus.UserCanceled);
        }

        public PaymentStatus GetPaymentStatus(PaymentLog paymentLog)
        {
            if (ErrorCode != PayGwErrorCode.Success)
            {
                return ErrorCode switch
                {
                    PayGwErrorCode.OrderProcessing => PaymentStatus.Pending,
                    PayGwErrorCode.UnpaidOrder => paymentLog.IsExpired ? PaymentStatus.Timeout : PaymentStatus.Pending,
                    PayGwErrorCode.PaymentExpired => PaymentStatus.Timeout,
                    _ => PaymentStatus.Unknown
                };
            }

            if (TransactionInfos == null || TransactionInfos.Count == 0)
            {
                return PaymentStatus.Pending;
            }

            if (TransactionInfos.Any(x => x.TransStatus == PayGwStatus.Paid))
            {
                return PaymentStatus.Success;
            }

            if (TransactionInfos.Count == 1)
            {
                return TransactionInfos[0].TransStatus switch
                {
                    PayGwStatus.NotPaid => paymentLog.IsExpired ? PaymentStatus.Timeout : PaymentStatus.Pending,
                    PayGwStatus.Pending => PaymentStatus.Pending,
                    PayGwStatus.Fail => PaymentStatus.Fail,
                    PayGwStatus.UserCanceled => PaymentStatus.Cancel,
                    PayGwStatus.WaitPartnerProcess => PaymentStatus.Pending,
                    _ => PaymentStatus.Unknown
                };
            }

            return ErrorCode switch
            {
                PayGwErrorCode.OrderProcessing => PaymentStatus.Pending,
                PayGwErrorCode.UnpaidOrder => paymentLog.IsExpired ? PaymentStatus.Timeout : PaymentStatus.Pending,
                _ => PaymentStatus.Unknown
            };
        }
    }

    public class PayGwTransactionInfo
    {
        public string OrderCode { get; set; }
        public string TransCode { get; set; }
        public long TotalAmount { get; set; }
        public long OrderAmount { get; set; }
        public long FeeAmount { get; set; }
        public long PaymentTime { get; set; }
        public long? TimeLimit { get; set; }
        public PayGwPaymentMethod PaymentMethod { get; set; }
        public PayGwStatus TransStatus { get; set; }
    }
}
