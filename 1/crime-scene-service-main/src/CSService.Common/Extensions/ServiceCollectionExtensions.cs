using CSService.Common.Authorization;
using CSService.Common.Authorization.Impl;
using CSService.Common.Buffers;
using CSService.Common.Buffers.Impl;
using CSService.Common.DataAccess;
using CSService.Common.DataAccess.Impl;
using CSService.Common.FileAccess;
using CSService.Common.FileAccess.Impl;
using CSService.Common.Services;
using CSService.Common.Services.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace CSService.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services) {
        services
            .AddSingleton<IConnectionFactory, SqlLiteConnectionFactory>()
            .AddSingleton<IPasswordHashService, PasswordHashService>()
            .AddSingleton<IFileStorage, FileStorage>()
            .AddSingleton<IPhotoCompressionService, PhotoCompressionService>()
            .AddSingleton<IVoiceRecognitionService, VoiceRecognitionService>()
            .AddSingleton<IDocxService, DocxService>()
            .AddSingleton<IBuffer, Buffer>();

        services.AddScoped<IVrHeadsetContext, VrHeadsetContext>();

        services.AddTransient<ISqlRepository, SqlRepository>();

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }
}
