using System.Security.Cryptography;

namespace AuthApi.Services;

public sealed class SaltGeneratorService
{
    public string GenerateSalt()
    {
        var saltBytes = RandomNumberGenerator.GetBytes(64);
        var salt = Convert.ToBase64String(saltBytes);

        return salt;
    }
}