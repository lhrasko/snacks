using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnacksPOS.Domain;
using SnacksPOS.Infrastructure.Services;

namespace SnacksPOS.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(config.GetConnectionString("Default") ?? "Data Source=snacks.db"));
        services.AddIdentity<ApplicationUser, IdentityRole>(o =>
        {
            o.SignIn.RequireConfirmedAccount = false;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<IEmailSender, ConsoleEmailSender>();
        services.AddHostedService<MonthlyReminderService>();
        return services;
    }
}

