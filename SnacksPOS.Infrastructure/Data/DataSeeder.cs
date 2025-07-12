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

        // Create roles first
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!context.Users.Any())
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<SnacksPOS.Infrastructure.ApplicationUser>>();
            
            // Create admin user
            var admin = new SnacksPOS.Infrastructure.ApplicationUser 
            { 
                UserName = "admin", 
                Email = "admin@example.com",
                EmailConfirmed = true  // Add this to avoid email confirmation issues
            };
            var adminResult = await userMgr.CreateAsync(admin, "password");
            if (adminResult.Succeeded)
            {
                await userMgr.AddToRoleAsync(admin, "Admin");
                Console.WriteLine("Admin user created successfully");
            }
            else
            {
                Console.WriteLine($"Failed to create admin user: {string.Join(", ", adminResult.Errors.Select(e => e.Description))}");
            }

            // Create demo user
            var user = new SnacksPOS.Infrastructure.ApplicationUser 
            { 
                UserName = "demo", 
                Email = "demo@example.com",
                EmailConfirmed = true  // Add this to avoid email confirmation issues
            };
            var demoResult = await userMgr.CreateAsync(user, "password");
            if (demoResult.Succeeded)
            {
                Console.WriteLine("Demo user created successfully");
            }
            else
            {
                Console.WriteLine($"Failed to create demo user: {string.Join(", ", demoResult.Errors.Select(e => e.Description))}");
            }
        }

        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "Turbo Trail-Mix", Description = "Energize!", Price = 1.50m, Stock = 100 },
                new Product { Name = "Power-Up Protein Bar", Description = "Level up!", Price = 2.00m, Stock = 100 },
                new Product { Name = "Mystery Chips", Description = "Surprise flavor", Price = 1.00m, Stock = 100 }
            );
            await context.SaveChangesAsync();
        }
    }
}
