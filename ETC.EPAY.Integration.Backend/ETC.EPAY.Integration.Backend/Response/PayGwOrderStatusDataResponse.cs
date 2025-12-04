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
            transactionInfo = TransactionInfos?.FirstOrDefault(x => x.TransStatus == TransStatus.Paid);
            return transactionInfo != null;
        }

        public PayGwTransactionInfo? GetFinalTransactionInfo()
        {
            return TransactionInfos?.FirstOrDefault(x => x.TransStatus is TransStatus.Paid or TransStatus.Fail or TransStatus.UserCanceled);
        }

        public TransStatus GetPaymentStatus(PaymentLog paymentLog)
        {
            if (ErrorCode != PayGwErrorCode.Success)
            {
                return ErrorCode switch
                {
                    PayGwErrorCode.OrderProcessing => TransStatus.Pending,
                    PayGwErrorCode.UnpaidOrder => paymentLog.IsExpired ? TransStatus.Fail : TransStatus.Pending,
                    _ => TransStatus.Pending
                };
            }

            if (TransactionInfos == null || TransactionInfos.Count == 0)
            {
                return TransStatus.Pending;
            }

            if (TransactionInfos.Any(x => x.TransStatus == TransStatus.Paid))
            {
                return TransStatus.Paid;
            }

            if (TransactionInfos.Count == 1)
            {
                return TransactionInfos[0].TransStatus switch
                {
                    TransStatus.NotPaid => paymentLog.IsExpired ? TransStatus.Fail : TransStatus.Pending,
                    TransStatus.Pending => TransStatus.Pending,
                    TransStatus.Fail => TransStatus.Fail,
                    TransStatus.UserCanceled => TransStatus.Fail,
                    TransStatus.WaitPartnerProcess => TransStatus.Pending,
                    _ => TransStatus.Pending
                };
            }

            return ErrorCode switch
            {
                PayGwErrorCode.OrderProcessing => TransStatus.Pending,
                PayGwErrorCode.UnpaidOrder => paymentLog.IsExpired ? TransStatus.Fail : TransStatus.Pending,
                _ => TransStatus.Pending
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
        public PaymentMethod PaymentMethod { get; set; }
        public TransStatus TransStatus { get; set; }
    }
}
