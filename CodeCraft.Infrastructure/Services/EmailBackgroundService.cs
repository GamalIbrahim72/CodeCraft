using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace CodeCraft.Infrastructure.Services;

public class EmailBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public EmailBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[Reminder] EmailBackgroundService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            var targetHour = int.Parse(_configuration["ReminderEmail:Hour"] ?? "21");
            var targetMinute = int.Parse(_configuration["ReminderEmail:Minute"] ?? "0");

            var now = DateTime.Now;

            var nextRun = new DateTime(
                now.Year,
                now.Month,
                now.Day,
                targetHour,
                targetMinute,
                0);

            if (now >= nextRun)
                nextRun = nextRun.AddDays(1);

            var delay = nextRun - now;

            Console.WriteLine($"[Reminder] Now: {now}");
            Console.WriteLine($"[Reminder] Next run: {nextRun}");
            Console.WriteLine($"[Reminder] Waiting: {delay}");

            await Task.Delay(delay, stoppingToken);

            using var scope = _serviceProvider.CreateScope();

            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var userTrackRepo = scope.ServiceProvider.GetRequiredService<IUserTrackRepository>();

            var users = await userTrackRepo.GetEnrolledUsersAsync();

            Console.WriteLine($"[Reminder] Users count: {users.Count()}");

            foreach (var user in users)
            {
                try
                {
                    await emailService.SendEmailAsync(
                        user.Email,
                        "CodeCraft Reminder 🔥",
                        $"متنساش تكمل التراك بتاعك {user.FirstName} 💪"
                    );

                    Console.WriteLine($"[Reminder] Sent to {user.Email}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Reminder] Failed to send to {user.Email}: {ex.Message}");
                }
            }
        }
    }
}