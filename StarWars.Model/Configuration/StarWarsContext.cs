using Microsoft.EntityFrameworkCore;
using StarWars.Model.Entities;

namespace StarWars.Model.Configuration;

public class StarWarsContext: DbContext
{
    public DbSet<Character> Characters { get; set; }

    private readonly string _dbPath;
    
    public StarWarsContext()
    {
        var currentDir = Environment.CurrentDirectory;
        var projectDir = Directory.GetParent(currentDir)!.FullName;
        _dbPath = Path.Combine(projectDir, "DB", "starwarsDB.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_dbPath}");
}