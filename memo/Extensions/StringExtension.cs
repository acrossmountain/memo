﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace Memo.Extensions
{
    public static class StringExtension
    {
        public static string GetMD5(this string data)
        {
            if (string.IsNullOrWhiteSpace(data)) throw new ArgumentNullException(nameof(data));
            var hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }
}
