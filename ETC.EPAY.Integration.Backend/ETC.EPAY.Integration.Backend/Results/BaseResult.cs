using ETC.EPAY.Integration.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Results
{
    public class BaseResult<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; init; }

        [JsonPropertyName("status")]
        [JsonIgnore]
        public StatusEnum Status { get; init; }

        [JsonPropertyName("statusCode")]
        public string StatusCode { get; init; }

        [JsonPropertyName("message")]
        public string Message { get; init; }

        public BaseResult() { }

        public BaseResult(string message)
        {
            Data = default;
            Status = StatusEnum.Failed;
            Message = message;
        }
    }
}
