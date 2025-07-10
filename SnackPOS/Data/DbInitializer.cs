using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnackPOS.Models;

namespace SnackPOS.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if(!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var admin = await userManager.FindByNameAsync("admin");
        if(admin == null)
        {
            admin = new IdentityUser { UserName = "admin" };
            await userManager.CreateAsync(admin, "Password1!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        if(!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "Chips", Price = 1.50m },
                new Product { Name = "Soda", Price = 1.00m },
                new Product { Name = "Candy", Price = 0.75m }
            );
            await context.SaveChangesAsync();
        }
    }
}
