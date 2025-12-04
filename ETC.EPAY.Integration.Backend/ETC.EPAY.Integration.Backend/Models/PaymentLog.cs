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
        public string Id { get; set; }
        public string client_ip { get; set; }
        public string trace_id { get; set; }
        public int transaction_id { get; set; }
        public PaymentType payment_type { get; set; }
        public DateTime request_datetime_utc { get; set; }
        public string partner_transaction_id { get; set; }
        public PaymentStatus partner_payment_status { get; set; }
        public string pos_id { get; set; }
        public string pos_ip { get; set; }
        public string device_id { get; set; }
        public string device_name { get; set; }
        public string order_id { get; set; }
        public DateTime response_datetime_utc { get; set; }
        public DateTime expired_datetime_utc { get; set; }
        public string qr { get; set; }
        public PaymentFlow payment_flow { get; set; }
        public string phone_number { get; set; }
        public string currency_code { get; set; }
        public string invoice { get; set; }
        public string ma_ho_so { get; set; }
        public string ma_nb { get; set; }
        public string ma_phieu_thu { get; set; }
        public PaymentStatus isofh_payment_status { get; set; }

        public decimal new_total_amount { get; set; }
        public decimal old_total_amount { get; set; }
        public string his_register { get; set; }
        public DateTime created_datetime_utc { get; set; }
        public DateTime updated_datetime_utc { get; set; }

        #region Phục vụ phần retry tự động
        public DateTime? last_retry_datetime_utc { get; set; }
        public int? total_retry { get; set; }

        public string payment_url { get; set; }

        #endregion

        public bool IsExpired => DateTime.UtcNow > expired_datetime_utc;
    }
}
