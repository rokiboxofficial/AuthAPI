using Microsoft.EntityFrameworkCore;

namespace AuthApi.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext()
    {
        Database.EnsureCreated();
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    public virtual DbSet<User> Users => Set<User>();
}