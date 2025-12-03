using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System.Security.Cryptography;
using System.Text;
using global::ETC.EPAY.Integration.Resources.Enums;
using Newtonsoft.Json;

namespace ETC.EPAY.Integration.Extensions
{
    public static class EncryptionHelper
    {
        // AES
        private const int GcmIvNonceSizeBytes = 12;
        private const int Pbkdf2SaltSizeBytes = 32;
        private const int Pbkdf2Iterations = 65536;
        private static readonly SecureRandom Random = new();

        private const byte GcmTagSize = 16; // in bytes
        private const CipherModeAES CipherMode = CipherModeAES.GCM;

        private const string Transformation = "AES/GCM/NoPadding";
        private static readonly HashAlgorithmName HashAlg = HashAlgorithmName.SHA256;

        public static string Sha256Hash(this string plainText)
        {
            using SHA256 mySha256 = SHA256.Create();
            byte[] plainTextData = Encoding.UTF8.GetBytes(plainText);
            byte[] hashValue = mySha256.ComputeHash(plainTextData);
            return Convert.ToBase64String(hashValue);
        }

        /// <summary>
        /// Chức năng: mã hoá string bằng AES
        /// </summary>
        /// <param name="plainText">bản tin cần mã hoá</param>
        /// <param name="secretKey">Khóa bí mật</param>
        /// <returns></returns>
        public static string AesEncrypt(this string plainText, string secretKey)
        {
            byte[] salt = GenerateRandomArray(Pbkdf2SaltSizeBytes);
            byte[] key = GetKey(secretKey, salt);
            byte[] iv = GenerateRandomArray(GcmIvNonceSizeBytes);
            var keyParameters = CreateKeyParameters(key, iv, GcmTagSize * 8);
            var cipher = CipherUtilities.GetCipher(Transformation);
            cipher.Init(true, keyParameters);

            byte[] plainTextData = Encoding.UTF8.GetBytes(plainText);
            byte[]? cipherText = cipher.DoFinal(plainTextData);

            byte[] result = Arrays.CopyOf(salt, GcmIvNonceSizeBytes + Pbkdf2SaltSizeBytes + cipherText.Length);
            Array.Copy(iv, 0, result, Pbkdf2SaltSizeBytes, GcmIvNonceSizeBytes);
            Array.Copy(cipherText, 0, result, GcmIvNonceSizeBytes + Pbkdf2SaltSizeBytes, cipherText.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Chức năng: giải mã string bằng AES
        /// </summary>
        /// <param name="cipherText">bản tin mã hoá</param>
        /// <param name="secretKey">Khóa bí mật</param>
        /// <returns></returns>
        public static string AesDecrypt(this string cipherText, string secretKey)
        {
            (byte[] encryptedBytes, byte[] iv, byte[] salt) = UnpackCipherData(cipherText);
            byte[] key = GetKey(secretKey, salt);
            var keyParameters = CreateKeyParameters(key, iv, GcmTagSize * 8);
            var cipher = CipherUtilities.GetCipher(Transformation);
            cipher.Init(false, keyParameters);

            byte[]? decryptedData = cipher.DoFinal(encryptedBytes);
            return Encoding.UTF8.GetString(decryptedData);
        }


        /// <summary>
        /// Chức năng: mã hoá string bằng AES không dùng salt (hỗ trợ 128-bit)
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AesWithoutSaltEncrypt(this string plainText, string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(plainText))
                return string.Empty;

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream =
                           new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// Chức năng: mã hoá string bằng AES không dùng salt. Tự hash key bằng thuật toán MD5 (hỗ trợ 128-bit)
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AesWithoutSaltEncrypt(this object obj, string key, bool useMd5 = false)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            var plainText = JsonConvert.SerializeObject(obj);
            return useMd5 ? plainText.AesWithoutSaltEncrypt(key.CreateMD5()) : plainText.AesWithoutSaltEncrypt(key);
        }

        /// <summary>
        /// Chức năng: giải mã string bằng AES không dùng salt (hỗ trợ 128-bit)
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AesWithoutSaltDecrypt(this string cipherText, string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(cipherText))
                return string.Empty;

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream =
                           new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        private static byte[] GenerateRandomArray(int sizeInBytes)
        {
            byte[] randomArray = new byte[sizeInBytes];
            Random.NextBytes(randomArray);
            return randomArray;
        }

        private static byte[] GetKey(string secretKey, byte[] salt)
        {
            var key = Rfc2898DeriveBytes.Pbkdf2(secretKey, salt, Pbkdf2Iterations, HashAlg, Pbkdf2SaltSizeBytes);

            return key;
        }

        private static ICipherParameters CreateKeyParameters(byte[] key, byte[] iv, int macSize)
        {
            var keyParameter = new KeyParameter(key);
            if (CipherMode == CipherModeAES.CBC)
            {
                return new ParametersWithIV(keyParameter, iv);
            }
            else if (CipherMode == CipherModeAES.GCM)
            {
                return new AeadParameters(keyParameter, macSize, iv);
            }

            throw new Exception("Lỗi AES-mode không hỗ trợ");
        }

        private static (byte[], byte[], byte[]) UnpackCipherData(string cipherText)
        {
            byte[] bytes = Convert.FromBase64String(cipherText);
            byte[] salt = Arrays.CopyOfRange(bytes, 0, Pbkdf2SaltSizeBytes);
            byte[] iv = Arrays.CopyOfRange(bytes, Pbkdf2SaltSizeBytes,
                Pbkdf2SaltSizeBytes + GcmIvNonceSizeBytes);
            byte[] encryptedBytes =
                Arrays.CopyOfRange(bytes, GcmIvNonceSizeBytes + Pbkdf2SaltSizeBytes, bytes.Length);

            return (encryptedBytes, iv, salt);
        }

    }
}
