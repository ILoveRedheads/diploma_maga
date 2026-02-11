using CSService.Queries.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace CSService.Queries.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services) {
        services.AddScoped<IExaminerQueries, ExaminerQueries>();
        services.AddScoped<IVrHeadsetQueries, VrHeadsetsQueries>();
        services.AddScoped<ISceneQueries, SceneQueries>();
        services.AddScoped<ISessionQueries, SessionQueries>();
        services.AddScoped<ISessionQueries, SessionQueries>();

        return services;
    }
}
