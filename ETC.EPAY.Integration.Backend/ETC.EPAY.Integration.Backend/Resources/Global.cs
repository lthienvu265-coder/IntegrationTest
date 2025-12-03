using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources
{
    public static class Constant
    {
        public const string DefaultAuthScheme = "Bearer";
        public const string ApiKeyAuthScheme = "ApiKey";
        public const int IterationCount = 10000; // Iteration Count for hashing pwd
        public const int TimeOutCancelDAO = 5000; // Ms
        public const int TimeOutCancelC06 = 20000; // Ms
        public const int TimeOutCancelEtcSc = 10000; // Ms
        public const int TimeOutCancelNursingApp = 10000; // Ms
        public const int TimeOutCancelACV = 10000; // Ms
        public const int TimeOutCancelECR = 59000; // Ms
        public const int TimeOutCancelIsofh = 20000; // Ms
        public const int TimeOutCancelEpayWallet = 20000; // Ms
        public const int TimeOutCancelQrGateway = 20000; // Ms
        public const int TimeOutCancelGOV = 10000; // Ms
        public const string TimeZoneId = "SE Asia Standard Time"; // Bangkok, Hanoi, Jakarta
        public const string PrefixEpayPayment = "EPAY000"; // Tiền tố mã giao dịch Epay
        public const string PrefixPatientApp = "xp_mobile_"; // Tiền tố file ứng dụng áp bệnh nhân
        public const string PathAppVersionFiles = "/files/app_version";
    }
    public static class Global
    {
        public static string ConnectionString { get; set; }
        public static string Version { get; set; }
        public static bool IsDebug { get; set; }
        public static string ImageLogPath { get; set; }
        public static string PatientImageLogPath { get; set; }
        public static string SecretKey { get; set; }
        public static string PayGwToken { get; set; }
        public static string QmsToken { get; set; }
    }

    public sealed class QrGateway
    {
        public static string RSAPublicKeyQrGateway { get; set; } // RSA-2048: Khoá public phía QrGateway cung cấp
        public static string RSAPublicKey { get; set; } // RSA-2048: Khoá public phía BE
        public static string RSAPrivateKey { get; set; } // RSA-2048: Khoá private phía BE
        public static string SecretKey { get; set; }
        public static string MerchantCode { get; set; }
        public static string TypeQR { get; set; } // DYNAMIC - STATIC
        public static string Content { get; set; } // Nội dung thực hiện thanh toán
        public static int ExpireTime { get; set; } // Thời gian hết hạn của QR
    }

    public class PaymentGateway
    {
        public static string Login = "/gw/api/paymentgateway/merchant/token";
        public static string CreateOrder = "/gw/api/paymentgateway/merchant/create_order";
        public static string CheckStatus = "/gw/api/paymentgateway/merchant/check_status";
        public static string Refund = "/gw/api/paymentgateway/merchant/refund";
        public static string CheckRefundStatus = "/gw/api/paymentgateway/merchant/check_refund_status";
        public static string DeleteToken = "/gw/api/paymentgateway/merchant/delete_token";
        public static string RetrieveToken = "/gw/api/paymentgateway/merchant/retrieve_token";
        public static string QrCusPayment = "/gw/api/paymentgateway/merchant/qr-cus-payment";
    }
}
