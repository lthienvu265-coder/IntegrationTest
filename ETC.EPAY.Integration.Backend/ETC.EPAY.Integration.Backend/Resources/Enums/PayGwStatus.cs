using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources.Enums
{
    public enum TransStatus
    {
        NotPaid = 0,
        Paid = 1,
        Fail = 2,
        Pending = 3,
        UserCanceled = 4,
        WaitPartnerProcess = 5
    }
}
