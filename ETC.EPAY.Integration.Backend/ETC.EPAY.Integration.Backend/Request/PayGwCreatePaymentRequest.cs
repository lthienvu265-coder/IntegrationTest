namespace ETC.EPAY.Integration.Backend.Request
{
    public class PayGwCreatePaymentRequest
    {
        public string OrderCode { get; set; }
        public int PackageType { get; set; }
        public string MemberName { get; set; }
        public string MemberPhoneNumber { get; set; }
        public long TotalAmount { get; set; }

        public string DiscountCode { get; set; }
        public long NeededAmount { get; set; }
        public int PaymentMethod { get; set; }
        public string DeviceId { get; set; }
    }
}
