using ETC.EPAY.Integration.Extensions;
using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Models
{
    public partial class PaymentLog
    {
        [Key]
        [Column("ID")]
        [StringLength(38)]
        public string Id { get; set; }

        [Required]
        [Column("CLIENT_IP")]
        [StringLength(45)]
        public string ClientIp { get; set; }

        [Required]
        [Column("TRACE_ID")]
        [StringLength(50)]
        public string TraceId { get => traceId; set => traceId = value.ReplaceChar(); }
        private string traceId;

        [Column("TRANSACTION_ID", TypeName = "NUMBER")]
        public int TransactionId { get; set; }

        [Column("PAYMENT_TYPE")]
        public PaymentType PaymentType { get; set; }

        [Column("REQUEST_DATETIME_UTC")]
        public DateTime RequestDatetimeUtc { get; set; }

        [Column("PARTNER_TRANSACTION_ID")]
        [StringLength(38)]
        public string PartnerTransactionId { get; set; }

        [Column("PARTNER_PAYMENT_STATUS")]
        public PaymentStatus PartnerPaymentStatus { get; set; }

        [Column("POS_ID")]
        [StringLength(50)]
        public string PosId { get; set; }

        [Column("POS_IP")]
        [StringLength(45)]
        public string PosIp { get; set; }

        [Required]
        [Column("DEVICE_ID")]
        [StringLength(50)]
        public string DeviceId { get; set; }

        [Required]
        [Column("DEVICE_NAME")]
        [StringLength(200)]
        public string DeviceName { get; set; }

        [Required]
        [Column("ORDER_ID")]
        [StringLength(38)]
        public string OrderId { get; set; }

        [Column("RESPONSE_DATETIME_UTC")]
        public DateTime ResponseDatetimeUtc { get; set; }

        [Column("EXPIRED_DATETIME_UTC")]
        public DateTime ExpiredDatetimeUtc { get; set; }

        [Column("QR")]
        [StringLength(200)]
        public string Qr { get; set; }

        [Column("PAYMENT_FLOW")]
        public PaymentFlow PaymentFlow { get; set; }

        [Column("PHONE_NUMBER")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Column("CURRENCY_CODE")]
        [StringLength(20)]
        public string CurrencyCode { get; set; }

        [Column("INVOICE")]
        [StringLength(100)]
        public string Invoice { get; set; }

        [Required]
        [Column("MA_HO_SO")]
        [StringLength(50)]
        public string MaHoSo { get; set; }

        [Required]
        [Column("MA_NB")]
        [StringLength(50)]
        public string MaNb { get; set; }

        [Required]
        [Column("MA_PHIEU_THU")]
        [StringLength(50)]
        public string MaPhieuThu { get; set; }

        [Column("ISOFH_PAYMENT_STATUS")]
        public PaymentStatus IsofhPaymentStatus { get; set; }

        [Column("NEW_TOTAL_AMOUNT", TypeName = "NUMBER(12,2)")]
        public decimal NewTotalAmount { get; set; }

        [Column("OLD_TOTAL_AMOUNT", TypeName = "NUMBER(12,2)")]
        public decimal OldTotalAmount { get; set; }

        [Column("HIS_REGISTER", TypeName = "CLOB")]
        public string HisRegister { get; set; }

        [Column("CREATED_DATETIME_UTC")]
        public DateTime CreatedDatetimeUtc { get; set; }

        [Column("UPDATED_DATETIME_UTC")]
        public DateTime UpdatedDatetimeUtc { get; set; }

        #region Phục vụ phần retry tự động

        [Column("LAST_RETRY_DATETIME_UTC")]
        public DateTime? LastRetryDatetimeUtc { get; set; }

        [Column("TOTAL_RETRY")]
        public int? TotalRetry { get; set; }

        [Column("PAYMENT_URL", TypeName = "CLOB")]
        public string PaymentUrl { get; set; }

        #endregion

        public bool IsExpired => DateTime.UtcNow > ExpiredDatetimeUtc;
    }
}
