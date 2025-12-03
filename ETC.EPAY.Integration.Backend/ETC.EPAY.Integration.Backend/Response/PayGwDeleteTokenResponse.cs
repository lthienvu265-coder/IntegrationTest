using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Response
{
    public class PayGwDeleteTokenResponse : PayGwDataResponseBase
    {
        public string Checksum { get; set; }
    }
}
