using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Application.Services;
using CodeCraft.Infrastructure.Persistence;
using CodeCraft.Infrastructure.Repositories;
using CodeCraft.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.DependencyInjection;
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
       this IServiceCollection services,
       IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")
            ));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITrackRepository, TrackRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IUserTrackRepository, UserTrackRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITrackChatService, TrackChatService>();

       // services.AddHostedService<EmailBackgroundService>();


        return services;
    }
}
