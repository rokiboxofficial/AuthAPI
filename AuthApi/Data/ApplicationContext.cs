using Microsoft.EntityFrameworkCore;

namespace AuthApi.Data;

public class ApplicationContext : DbContext
{
    private readonly string _databasePath;

    public ApplicationContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        _databasePath = System.IO.Path.Join(path, "application.db");

        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_databasePath}");
}
