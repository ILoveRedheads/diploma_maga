using FluentMigrator.Runner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CSService.API.Tests;

public sealed class CrimeSceneWebApplicationFactory : WebApplicationFactory<Program>
{
    public string DatabasePath { get; } = Path.Combine(Path.GetTempPath(), $"crime-scene-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) => {
            config.AddInMemoryCollection(new Dictionary<string, string?> {
                ["ConnectionStrings:CSService"] = $"Data Source={DatabasePath}"
            });
        });

        builder.ConfigureServices(services => {
            services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString($"Data Source={DatabasePath}")
                    .ScanIn(typeof(CSService.Migrations.Program).Assembly).For.Migrations());
        });
    }

    protected override IHost CreateHost(IHostBuilder builder) {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
        return host;
    }
}
