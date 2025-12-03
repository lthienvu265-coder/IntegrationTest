using ETC.EPAY.Integration.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Response
{
    public class PayGwResponse<T>
    {
        public string MerchantCode { get; set; }
        public string Signature { get; set; }
        public string Data { get; set; }

        public bool VerifySignature(string publicKey, string secretKey)
        {
            return $"{Data}{secretKey}".RsaVerify(Signature, publicKey);
        }

        public T? DecryptedData(string secretKey)
        {
            string decryptedText = Data.AesDecrypt(secretKey);
            return JsonConvert.DeserializeObject<T>(decryptedText);
        }
    }
}
