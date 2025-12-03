using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Resources
{
    public sealed class ResponseMessage
    {
        #region Property
        public Dictionary<string, string> Values { get; set; }
        #endregion
    }

    public enum CodeMessage
    {
        _95,
        _97,
        _98,
        _99,
        _101,
        _102,
        _103,
        _104,
        _105,
        _106,
        _107,
        _108,
        _109,
        _200,
        _209,
        _210,
        _211,
        _212,
        _213,
        _214,
        _218,
        _228,
        _227,
        _229,
        _230,
        _233,
        _234,
        _235,
        _236,
        _237,
        _239,
        _241,
        _242,
        _243,
        _244,
        _245,
        _246,
        _247,
        _248,
        _249,
        _250,
        _251,
        _300,
        _303,
        _304,
        _320,
        _330,
        _332,
        _333,
        _334,
        _335,
        _336,
        _337,
        _338,
        _339,
        _340,
        _341,
        _342,
        _344,
        _345,
        _346,
        _347,
        _348,
        _349,
        _350,
        _351,
        _352,
        _353,
        _401,
    }
}
