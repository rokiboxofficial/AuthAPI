using AuthApi.Configuration;
using AuthApi.Data;
using AuthApi.Extensions;
using Microsoft.EntityFrameworkCore;

internal sealed class Program
{
    private static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();
        builder.Services.AddServices();
        builder.Services.AddTransient<AuthTokensConfiguration>();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationContext>(options
            => options.UseNpgsql(connectionString));

        var app = builder.Build();        
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}