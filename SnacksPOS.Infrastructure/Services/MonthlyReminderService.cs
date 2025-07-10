using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnacksPOS.Domain;

namespace SnacksPOS.Infrastructure.Services;

public class MonthlyReminderService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<MonthlyReminderService> _logger;
    private readonly IEmailSender _email;

    public MonthlyReminderService(IServiceProvider sp, ILogger<MonthlyReminderService> logger, IEmailSender email)
    {
        _sp = sp;
        _logger = logger;
        _email = email;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var next = new DateTime(now.Year, now.Month, 1).AddMonths(1);
            var delay = next - now;
            await Task.Delay(delay, stoppingToken);
            await SendReminders(stoppingToken);
        }
    }

    private async Task SendReminders(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var unpaid = await db.LedgerEntries.Where(l => !l.Paid).ToListAsync(ct);
        var users = unpaid.GroupBy(u => u.UserId);
        foreach (var group in users)
        {
            var user = await userMgr.FindByIdAsync(group.Key);
            if (user == null) continue;
            var total = group.Sum(l => l.Total);
            await _email.SendAsync(user.Email ?? user.UserName!, "Snack balance reminder", $"You owe ₵{total:0.00}. Thanks!");
        }
        _logger.LogInformation("Monthly reminders sent");
    }
}
