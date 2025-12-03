using ETC.EPAY.Integration.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Request
{
    public class PayGwRequest<T>
    {
        public string MerchantCode { get; set; }
        public string Signature { get; set; }
        [JsonPropertyName("data")]
        public string RawData { get; set; }
        [JsonIgnore]
        public T? Data => JsonSerializer.Deserialize<T>(RawData);

        public static PayGwRequest<T> Create(T data, string merchantCode, string secretKey, string privateKey)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Bỏ qua các trường null
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Tùy chọn camelCase nếu cần
            };
            string jsonRaw = JsonSerializer.Serialize(data, options);
            string cipherText = jsonRaw.AesEncrypt(secretKey);
            string signature = $"{cipherText}{secretKey}".RsaGenerate(privateKey);

            return new PayGwRequest<T>
            {
                RawData = cipherText,
                Signature = signature,
                MerchantCode = merchantCode
            };
        }
    }
}
