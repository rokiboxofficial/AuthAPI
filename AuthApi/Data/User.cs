using Microsoft.EntityFrameworkCore;

namespace AuthApi.Data;

[Index(nameof(Login), IsUnique = true)]
public sealed class User
{
    public int Id { get; set; }
    public string Login { get; set;} = null!;
    public string PasswordHash { get; set; } = null!;
    public string Salt { get; set; } = null!;

    public User()
    {

    }

    public User(string login, string passwordHash, string salt)
    {
        Login = login;
        PasswordHash = passwordHash;
        Salt = salt;
    }
}