using System;
using System.Linq;
using System.Security.Cryptography;

namespace CSService.Common.Authorization.Impl;

internal sealed class PasswordHashService : IPasswordHashService
{
    private const short SIZE = 150;
    private const short ITERATIONS = 10;

    public HashedPassword Hash(string password) {
        var result = new HashedPassword();

        using var deriveBytes = new Rfc2898DeriveBytes(password, SIZE, ITERATIONS, HashAlgorithmName.SHA512);
        result.Salt = Convert.ToBase64String(deriveBytes.Salt);
        result.Hash = Convert.ToBase64String(deriveBytes.GetBytes(SIZE));

        return result;
    }

    public bool Verify(HashedPassword hashedPassword, string password) {
        var salt = Convert.FromBase64String(hashedPassword.Salt);
        var hash = Convert.FromBase64String(hashedPassword.Hash);

        using var deriveBytes = new Rfc2898DeriveBytes(password, salt, ITERATIONS, HashAlgorithmName.SHA512);
        byte[] newKey = deriveBytes.GetBytes(SIZE);

        return newKey.SequenceEqual(hash);
    }
}
