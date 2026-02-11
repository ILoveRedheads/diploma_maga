using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSService.API.Middlewares;
using CSService.API.Settings;
using CSService.API.Swagger;
using CSService.Common.Authorization;
using CSService.Common.DataAccess;
using CSService.Common.Extensions;
using CSService.Common.Services;
using CSService.Contracts;
using CSService.Queries.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CSService.API;

public class Program
{
    static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost
            .UseKestrel((context, options) => {
                options.Configure(context.Configuration.GetSection("Kestrel"));
                options.Limits.MaxRequestBodySize = 1024 * 1024 * 64;
            })
            .ConfigureAppConfiguration((context, configBuilder) => {
                var env = context.HostingEnvironment.EnvironmentName;

                configBuilder.AddJsonFile("appsettings.json");
                configBuilder.AddJsonFile($"appsettings.{env}.json", optional: true);
                configBuilder.AddJsonFile($"Configs/connectionStrings.{env}.json", optional: true);
                configBuilder.AddEnvironmentVariables();
            });

        builder.Services.AddAuthorization();
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new() {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                };
            });

        builder.Services.AddRouting();
        builder.Services.AddLogging();
        builder.Services.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All);
        builder.Services.AddResponseCompression();
        builder.Services.AddMvc();
        builder.Services
            .AddControllers(options => {
                options.Filters.Add(SupportsRestfulApi.Instance);
                options.Filters.Add(new ProducesResponseTypeAttribute(200));
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorDetails), 400));
            })
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => {
            options.AddSecurityDefinition(
                JwtBearerDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                        "Enter your token in the text input below.\r\n\r\n"
                });

            options.OperationFilter<AddSecurityRequirements<VrHeadsetAuthorizationAttribute>>();
            options.OperationFilter<AddAuthorizeResponses<VrHeadsetAuthorizationAttribute>>();

            options.AddSecurityRequirement(new() {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        In = ParameterLocation.Header,
                        Name = JwtBearerDefaults.AuthenticationScheme
                    },
                    Array.Empty<string>()
                }
            });

            var xmlDocs = Directory.GetFiles(AppContext.BaseDirectory, "*.xml").ToList();
            xmlDocs.ForEach(xmlDoc => options.IncludeXmlComments(xmlDoc));
            options.UseInlineDefinitionsForEnums();
        });

        builder.Services.AddSingleton<IConnectionSettings, AppSettings>();
        builder.Services.AddSingleton<IJwtSettings, AppSettings>();
        builder.Services.AddSingleton<IVoiceRegonitionSettings, AppSettings>();
        builder.Services.AddCommonServices();
        builder.Services.AddQueries();

        var app = builder.Build();

        if (app.Environment.EnvironmentName != "Production") {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseResponseCompression();
        app.UseHttpLogging();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.MapControllers();
        app.Map("/starts", ctx => ctx.Response.WriteAsync(DateTimeOffset.UtcNow.ToString("O")));
        app.Map("/ping", ctx => ctx.Response.WriteAsync("pong"));
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}