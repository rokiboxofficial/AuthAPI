using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AuthApi.Services;

public sealed class PasswordHashingService
{
    private const KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA1;
    private const int IterationCount = 10000;
    private const int NumberBytesRequested = 20;

    public string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);

        var hashedPassword = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(password, saltBytes, Prf, IterationCount,  NumberBytesRequested));
        
        return hashedPassword;
    }

    public bool ValidateHashedPassword(string password, string hashedPassword, string salt)
    {
        var passwordHash = HashPassword(password, salt);

        return passwordHash == hashedPassword;
    }
}