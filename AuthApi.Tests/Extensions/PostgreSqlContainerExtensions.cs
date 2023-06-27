using AuthApi.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

public static class PostgreSqlContainerExtensions
{
    public static ApplicationContext CreateApplicationContext(this PostgreSqlContainer postgreSqlContainer)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        var connectionString = postgreSqlContainer.GetConnectionString();
        var options = optionsBuilder.UseNpgsql(connectionString).Options;
        var applicationContext = new ApplicationContext(options);

        return applicationContext;
    }
}