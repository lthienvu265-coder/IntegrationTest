using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources.DTO.Payment.Request
{
    public class CheckPaymentEpayRequest
    {
        public string MaHoSo { get; set; }
        public string PhieuThuId { get; set; }
        public string MaNb { get; set; }
    }
}
