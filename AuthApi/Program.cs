using AuthApi.Data;
using AuthApi.Services;

internal sealed class Program
{
    private static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();
        builder.Services.AddDbContext<ApplicationContext>();
        builder.Services.AddTransient<JwtTokensFactoryService>();
        builder.Services.AddTransient<TokensRefreshingService>();
        var app = builder.Build();        
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}