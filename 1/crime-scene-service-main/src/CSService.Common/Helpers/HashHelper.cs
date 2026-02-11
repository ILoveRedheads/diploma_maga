using System;
using System.Security.Cryptography;
using System.Text;

namespace CSService.Common.Helpers;

public static class HashHelper
{
    public static string ComputeHash(string value) {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash);
    }
}
