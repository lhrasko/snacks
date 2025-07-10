using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SnacksPOS.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(config.GetConnectionString("Default") ?? "Data Source=snacks.db")
            .Options;
        return new AppDbContext(options);
    }
}
