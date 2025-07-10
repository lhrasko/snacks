using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SnacksPOS.Domain;

namespace SnacksPOS.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        if (!context.Users.Any())
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var admin = new ApplicationUser { UserName = "admin" };
            await userMgr.CreateAsync(admin, "password");
            await userMgr.AddToRoleAsync(admin, "Admin");

            var user = new ApplicationUser { UserName = "demo" };
            await userMgr.CreateAsync(user, "password");
        }

        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "Turbo Trail-Mix", Description = "Energize!", Price = 1.50m },
                new Product { Name = "Power-Up Protein Bar", Description = "Level up!", Price = 2.00m },
                new Product { Name = "Mystery Chips", Description = "Surprise flavor", Price = 1.00m }
            );
            await context.SaveChangesAsync();
        }
    }
}
