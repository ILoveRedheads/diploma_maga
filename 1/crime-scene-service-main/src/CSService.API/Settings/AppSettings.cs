using System;
using System.Text;
using CSService.Common.Authorization;
using CSService.Common.DataAccess;
using CSService.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CSService.API.Settings;

internal sealed class AppSettings(IConfiguration config) : IConnectionSettings, IJwtSettings, IVoiceRegonitionSettings
{
    public string ConnectionString { get; } = config.GetConnectionString("CSService");

    public string Issuer { get; } = config["JWT:Issuer"];

    public string Audience { get; } = config["JWT:Audience"];

    public string Key { get; } = config["JWT:Key"];

    public TimeSpan JwtLifeTime { get; } = config.GetValue("JWT:LifeTime", TimeSpan.FromHours(24));

    public string ModelPath { get; } = config["Model"];

    public SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new(Encoding.ASCII.GetBytes(Key));
}
