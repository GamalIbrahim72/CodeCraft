using CodeCraft.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CodeCraft.Application.DependencyInjection;
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITrackService, TrackService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<IProgressService, ProgressService>();
        services.AddScoped<IAdminService, AdminService>();

        services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);

        return services;
    }
}
