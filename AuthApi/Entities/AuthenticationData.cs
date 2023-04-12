namespace AuthApi.Entities;

public sealed class AuthenticationData
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Fingerprint { get; set; } = null!;
}