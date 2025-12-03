using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources.Enums
{
    public enum PayGwPaymentMethod
    {
        /// <summary>
        /// Sử dụng khi thanh toán POS
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Thanh toán qua ví EPAY
        /// </summary>
        EpayWallet = 1,

        /// <summary>
        /// Thanh toán qua thẻ ATM và tài khoản ngân hàng
        /// </summary>
        BankAccount = 2,

        /// <summary>
        /// Thanh toán qua thẻ tín dụng và ghi nợ quốc tế
        /// </summary>
        CreditCard = 3,

        /// <summary>
        /// Thanh toán qua ứng dụng Mobile Banking qua QR code
        /// </summary>
        QRMobileBanking = 4
    }
}
