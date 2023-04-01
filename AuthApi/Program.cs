internal sealed class Program
{
    private static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();

        var app = builder.Build();        
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}