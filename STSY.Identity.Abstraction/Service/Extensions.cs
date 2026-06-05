using System;

namespace STSY.Identity.Abstraction.Service
{
    public static class Extensions
    {
        public static string ToBase64(this byte[] value)
        {
            return Convert.ToBase64String(value);
        }
        public static byte[] Base64ToByte(this string value)
        {
            return Convert.FromBase64String(value);
        }

        public static string ToBase64Url(this byte[] value)
        {
            return value.ToBase64()
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static byte[] Base64UrlToByte(this string value)
        {
            var base64 = value
                .Replace('-', '+')
                .Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            return base64.Base64ToByte();
        }
    }
}
