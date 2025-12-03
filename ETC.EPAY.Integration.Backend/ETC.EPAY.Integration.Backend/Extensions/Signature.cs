using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.Extensions
{
    public static class Signature
    {
        public static bool RsaVerify(this string data, string dataSigned, string publicKey)
        {
            byte[] decoded = Convert.FromBase64String(publicKey.RemoveAllSpaceChar());

            AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(decoded);
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;

            ISigner signer = SignerUtilities.GetSigner("SHA1withRSA");

            signer.Init(false, rsaKeyParameters);

            var expectedSig = Convert.FromBase64String(dataSigned);

            var msgBytes = Encoding.UTF8.GetBytes(data);

            signer.BlockUpdate(msgBytes, 0, msgBytes.Length);
            return signer.VerifySignature(expectedSig);
        }

        public static string RsaGenerate(this string data, string privateKey)
        {
            byte[] decoded = Convert.FromBase64String(privateKey.RemoveAllSpaceChar());

            AsymmetricKeyParameter asymmetricKeyParameter = PrivateKeyFactory.CreateKey(decoded);
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;

            ISigner signer = SignerUtilities.GetSigner("SHA1withRSA");

            signer.Init(true, rsaKeyParameters);

            var bytes = Encoding.UTF8.GetBytes(data);

            signer.BlockUpdate(bytes, 0, bytes.Length);
            byte[] signature = signer.GenerateSignature();

            return Convert.ToBase64String(signature);
        }
    }
}
