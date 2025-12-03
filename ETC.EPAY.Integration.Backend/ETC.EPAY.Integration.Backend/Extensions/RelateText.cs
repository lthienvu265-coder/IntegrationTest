using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SkiaSharp.QrCode;

namespace ETC.EPAY.Integration.Extensions
{
    public static class RelateText
    {
        public static string GenerateQrBase64(this string inputData)
        {
            // Generate QrCode
            var qr = QRCodeGenerator.CreateQrCode(inputData, ECCLevel.Q);

            // Render to canvas
            var info = new SKImageInfo(512, 512);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Render(qr, info.Width, info.Height);

            // Output to Stream -> Base64
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return Convert.ToBase64String(data.AsSpan());
        }
        public static string ConvertTransactionIdToEpayId(this int transId) =>
           transId == 0 ? string.Empty : $"{Resources.Constant.PrefixEpayPayment}{transId}";

        public static int ConvertEpayIdToTransactionId(this string epayId)
        {
            if (string.IsNullOrEmpty(epayId))
                return 0;

            if (int.TryParse(epayId.Replace(Resources.Constant.PrefixEpayPayment, string.Empty), out int tempTransId))
                return tempTransId;

            return 0;
        }

        public static string UnAccent(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            source = source.Replace("Đ", "D").Replace("đ", "d");

            string formD = source.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).ToUpperAndRemoveSpace();
        }

        /// <summary>
        /// Chức năng: xoá các kí tự khoảng trắng bị lặp lại (2 kí tự space -> 1 kí tự space)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveSpaceCharacter(this string source) =>
            string.IsNullOrEmpty(source) ? string.Empty : Regex.Replace(source.Trim(), @"\s{2,}", " ");

        /// <summary>
        /// Chức năng: xoá kí tự khoảng trắng bị lặp và viết hoa tất cả
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToUpperAndRemoveSpace(this string source) =>
            RemoveSpaceCharacter(source).ToUpper();

        /// <summary>
        /// Chức năng: thay thế kí tự trong một chuỗi cho trước
        /// </summary>
        /// <param name="source"></param>
        /// <param name="old"></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        /// <summary>
        /// Chức năng: chuyển đổi kiểu dữ liệu decimal -> string (loại bỏ kí tự ".00" tận cùng nếu xuất hiện) <br/>
        /// Ex: 8.00 -> 8 (loại bỏ .00)<br/>
        /// 8.001 -> 8.001 (giữ nguyên giá trị)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DecimalToStringRemovePoint(this decimal input) =>
            Regex.Replace(input.ToString(), @"\.00$", string.Empty);

        public static string ReplaceChar(this string source, char old = ':', char @new = '_') =>
            string.IsNullOrEmpty(source) ? string.Empty : source.Replace(old, @new);

        /// <summary>
        /// Chức năng: làm sạch dữ liệu base64 trước khi sử dụng
        /// </summary>
        /// <param name="base64Data"></param>
        /// <returns></returns>
        public static string GetBase64Data(this string base64Data)
        {
            int base64Index = base64Data.IndexOf("base64,");
            string trimData = base64Data.RemoveSpaceCharacter();

            if (base64Index != -1)
                return trimData.Substring(base64Index + 7);
            else
                return trimData;
        }

        /// <summary>
        /// Chức năng: kiểm tra chuỗi truyền vào có phải base64-format
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static bool IsBase64String(this string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return false;

            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        /// <summary>
        /// Chức năng: loại bỏ toàn bộ kí tự khoảng trắng khỏi chuỗi
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveAllSpaceChar(this string text) =>
           string.IsNullOrEmpty(text) ? string.Empty : Regex.Replace(text.Trim(), @"\s+", "");

        /// <summary>
        /// Chức năng: format tiếng việt về chuẩn
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RenewAccentVietnamese(this string source)
        {
            source = !string.IsNullOrEmpty(source) ? source.ToLower() : string.Empty;

            source = Regex.Replace(source, @"(?:òa)", "oà");
            source = Regex.Replace(source, @"(?:óa)", "oá");
            source = Regex.Replace(source, @"(?:ỏa)", "oả");
            source = Regex.Replace(source, @"(?:õa)", "oã");
            source = Regex.Replace(source, @"(?:ọa)", "oạ");

            source = Regex.Replace(source, @"(?:òe)", "oè");
            source = Regex.Replace(source, @"(?:óe)", "oé");
            source = Regex.Replace(source, @"(?:ỏe)", "oẻ");
            source = Regex.Replace(source, @"(?:õe)", "oẽ");
            source = Regex.Replace(source, @"(?:ọe)", "oẹ");

            source = Regex.Replace(source, @"(?:ùy)", "uỳ");
            source = Regex.Replace(source, @"(?:úy)", "uý");
            source = Regex.Replace(source, @"(?:ủy)", "uỷ");
            source = Regex.Replace(source, @"(?:ũy)", "uỹ");
            source = Regex.Replace(source, @"(?:ụy)", "uỵ");

            return source;
        }
        public static string ExtractYear(this string date)
        {
            // Regex pattern để khớp với năm trong các định dạng khác nhau: yyyy-MM-dd, yyyy-MM, yyyy
            string pattern = @"^(\d{4})(?:-\d{2})?(?:-\d{2})?$";

            var regex = new Regex(pattern);

            var match = regex.Match(date);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        public static string ExtractMonth(this string date)
        {
            // Regex pattern để khớp với tháng trong các định dạng khác nhau: yyyy-MM-dd, yyyy-MM
            string pattern = @"^(?:\d{4})-(\d{2})(?:-\d{2})?$";

            var regex = new Regex(pattern);

            var match = regex.Match(date);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        public static string ExtractDay(this string date)
        {
            // Regex pattern để khớp với ngày trong định dạng yyyy-MM-dd
            string pattern = @"^(?:\d{4})-(?:\d{2})-(\d{2})$";

            var regex = new Regex(pattern);

            var match = regex.Match(date);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        public static string FormatBirthDate(int yearOfBirth, int? monthOfBirth, int? dateOfBirth)
        {
            // Kiểm tra xem năm sinh có hợp lệ không
            if (yearOfBirth <= 0)
            {
                return string.Empty;
            }

            // Tạo chuỗi kết quả
            string result = $"{yearOfBirth}";

            if (monthOfBirth.HasValue && monthOfBirth.Value > 0)
            {
                // Thêm tháng vào chuỗi kết quả với định dạng hai chữ số
                result += $"-{monthOfBirth.Value:D2}";
            }

            if (dateOfBirth.HasValue && dateOfBirth.Value > 0)
            {
                // Thêm ngày vào chuỗi kết quả với định dạng hai chữ số
                result += $"-{dateOfBirth.Value:D2}";
            }

            return result;
        }

        /// <summary>
        /// Chức năng: chuẩn hóa chuỗi thành chuỗi base32, dùng để tạo secret key cho OTP.NET
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string NormalizeToBase32(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be null or empty.");

            // Loại bỏ khoảng trắng và chuẩn hóa thành chữ hoa
            input = input.ToUpper().Replace(" ", "").Trim();

            // Lọc bỏ ký tự không hợp lệ (chỉ giữ A-Z và 2-7)
            input = Regex.Replace(input, "[^A-Z2-7]", "");

            // Nếu chuỗi rỗng sau khi lọc, gán giá trị mặc định
            if (input.Length == 0)
                input = "="; // Giá trị tối thiểu hợp lệ

            // Đảm bảo độ dài chia hết cho 8
            while (input.Length % 8 != 0)
            {
                input += "="; // Padding bằng 'A' để tránh randomness
            }

            return input;
        }

        public static string NameUpperFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var strArray = input.Split(' ');

            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].Length > 0)
                {
                    strArray[i] = char.ToUpper(strArray[i][0]) + strArray[i][1..].ToLower();
                }
            }

            return string.Join(' ', strArray);
        }
    }
}
