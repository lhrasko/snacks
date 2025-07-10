using Microsoft.Extensions.Logging;

namespace SnacksPOS.Infrastructure.Services;

public interface IEmailSender
{
    Task SendAsync(string userEmail, string subject, string message);
}

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;
    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger) => _logger = logger;

    public Task SendAsync(string userEmail, string subject, string message)
    {
        _logger.LogInformation("Email to {Email}: {Subject}\n{Message}", userEmail, subject, message);
        return Task.CompletedTask;
    }
}
