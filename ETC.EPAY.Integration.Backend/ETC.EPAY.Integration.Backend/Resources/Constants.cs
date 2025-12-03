using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources
{
    public static class Constants
    {
        public const string SslEnabledHttpClient = "SslEnabledHttpClient";
        public const string SslDisabledHttpClient = "SslDisabledHttpClient";
        public const string HidedMark = "*** Hidden data ***";
        public const string HisTokenKey = "xp-his-token";
        public const string ConfigFirebaseFileName = "firebase-adminsdk.json";
        public static class MechanismType
        {
            public const string SHA1_RSA = "sha1-rsa";
            public const string SHA256_RSA = "sha256-rsa";
            public const string SHA384_RSA = "sha384-rsa";
            public const string SHA512_RSA = "sha512-rsa";
        }

        public static class EtcScErrorCode
        {
            public const string Success = "200";
        }

        public static class PayGwMessageType
        {
            public const string GetToken = "token";
            public const string CreateOrder = "create_order";
            public const string CheckStatus = "check_status";
        }

        public static class PayGwChannelCode
        {
            public const string Website = "01";
            public const string MobileApp = "02";
            public const string POS = "03";
            public const string SmartPOS = "04";
            public const string StaticQR = "05";
            public const string Kiosk = "06";
            public const string MiniKiosk = "07";
            public const string SmartGate = "08";
        }

        public static class PatientRelationshipType
        {
            public const string Parent = "CHA ME";
            public const string Spouse = "VO CHONG";
            public const string Guardian = "NGUOI GIAM HO";
            public const string Other = "KHAC";
        }

        public static class Policy
        {
            public const string Admin = "Admin";
            public const string Patient = "Patient";
            public const string Device = "Device";
        }
        public static class NotificationTitle
        {
            public const string PaymentSuccess = "PaymentSuccess";
            public const string PaymentFailed = "PaymentFailed";
            public const string UpdateHisFail = "UpdateHisFail";
            public const string UpdateHisUnknown = "UpdateHisUnknown";
            public const string RegisterSuccess = "RegisterSuccess";
            public static readonly Dictionary<string, (string Title, string Body)> Titles = new()
        {
            { PaymentSuccess, ("Thanh toán thành công", "Bạn đã thanh toán thành công.") },
            { PaymentFailed, ("Thanh toán thất bại", "Thanh toán của bạn đã thất bại. Vui lòng thử lại.") },
            { UpdateHisFail, ("Thanh toán thành công", "Bạn đã thanh toán thành công nhưng đăng ký khám thất bại.") },
            { UpdateHisUnknown, ("Thanh toán thành công", "Bạn đã thanh toán thành công nhưng đăng ký khám thất bại.") },
            { RegisterSuccess, ("Đăng kí khám thành công", "Bạn đã đăng kí khám thành công.") },
        };
        }

        public static class AccountSource
        {
            public const string PatientApp = "PATIENT_APP";
            public const string Other = "OTHER";
        }
    }
}
