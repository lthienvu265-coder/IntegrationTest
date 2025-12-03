using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources.Enums
{
    public enum PaymentStatus
    {
        Fail = 0,
        Success = 1,
        Pending = 2,
        Cancel = 3,
        Init = 4,
        None = 5,
        Timeout = 6,
        Unknown = 7,
    }
}
