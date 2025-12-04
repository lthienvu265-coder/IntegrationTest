using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Request
{
    public class PayGwCreateOrderDataRequest : PayGwDataRequestBase
    {
        public string CustomerIdNumber { get; set; }
        public bool SaveToken { get; set; }
        public string BillCode { get; set; }
        public string AgencyCode { get; set; }
        public string AgencyName { get; set; }
        public string Provider { get; set; }
        public string MerchantPassword { get; set; }
        public string OrderCode { get; set; }
        public string OrderDescription { get; set; }
        public PayGwPaymentType PaymentType { get; set; }
        public string Currency { get; set; } = "VND";
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public string AgainUrl { get; set; }
        public string BusinessType { get; set; } = "0000";
        public long TotalAmount { get; set; }
        public long OrderAmount { get; set; }
        public long FeeAmount { get; set; } = 0;
        /// <summary>
        /// Thời gian cho phép thanh toán, tính theo phút, mặc định = 24 giờ (1440 phút)
        /// </summary>
        public int TimeLimit { get; set; } = 120;
        public string? CustomerFullName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerMobile { get; set; }
        public string? CustomerAddress { get; set; }
        public int TotalGoods { get; set; }
        public List<PayGwGoodDetail> DetailGoods { get; set; }
        /// <summary>
        /// Thông tin thêm
        /// </summary>
        public string? AddInfo { get; set; }
        /// <summary>
        /// Mã khách hàng của merchant (dùng mã bệnh nhân)
        /// </summary>
        public string? CustomerCode { get; set; }
        public string ChannelCode { get; set; }
        public string PaymentMethod { get; set; }
        public string KioskId { get; set; }
        public string DeviceId { get; set; }
        public string? PosSerial { get; set; }
        public string? PosRefId { get; set; }
        public string? PosMerchantId { get; set; }
        public string? PosClientId { get; set; }
        public string? PosTerminalId { get; set; }
        public string? PosMerchantOutletId { get; set; }
        public PayGwWalletFunctionType? WalletFunctionType { get; set; }
        public bool IsCardReader { get; set; }
        public PayGwCardInfo? CardInfo { get; set; }
        public string? TypeCardAccount { get; set; }
    }

    public class PayGwGoodDetail
    {
        public string GoodsName { get; set; }
        public int GoodsQuantity { get; set; }
        public long GoodsPrice { get; set; }
        public string? GoodsUrl { get; set; }
        public string GoodCode { get; set; }
    }

    public class PayGwCardInfo
    {
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public string IssuedDate { get; set; }
    }
}
