using AuthApi.Data;

internal sealed class Program
{
    private static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();
        builder.Services.AddDbContext<ApplicationContext>();
        var app = builder.Build();        
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}