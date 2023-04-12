using AuthApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Services;

public sealed class UserFinderService
{
    private readonly ApplicationContext _applicationContext;

    public UserFinderService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<User?> FindAsync(string login)
        => await _applicationContext.Users.FirstOrDefaultAsync(user => user.Login == login);
}