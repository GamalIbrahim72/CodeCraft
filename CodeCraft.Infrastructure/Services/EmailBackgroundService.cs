using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Services;
public class EmailBackgroundService: BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    

    public EmailBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var userTrackRepo = scope.ServiceProvider.GetRequiredService<IUserTrackRepository>();

            var users = await userTrackRepo.GetEnrolledUsersAsync();

            foreach (var user in users)
            {
                await emailService.SendEmailAsync(
                    user.Email,
                    "CodeCraft Reminder 🔥",
                    $"متنساش تكمل التراك بتاعك  {user.FirstName} 💪"
                );
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}

