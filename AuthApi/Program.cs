using AuthApi.Data;
using AuthApi.Extensions;

internal sealed class Program
{
    private static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();
        builder.Services.AddServices();
        builder.Services.AddDbContext<ApplicationContext>();

        var app = builder.Build();        
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}