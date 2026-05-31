using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CSService.Common.Authorization;
using CSService.Contracts.Examiners;
using Xunit;

namespace CSService.API.Tests;

public sealed class ApiIntegrationTests : IClassFixture<CrimeSceneWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(CrimeSceneWebApplicationFactory factory) {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Ping_ReturnsPong() {
        var response = await _client.GetAsync("/ping");
        response.EnsureSuccessStatusCode();
        Assert.Equal("pong", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Register_And_Login_ReturnNonEmptyToken() {
        var login = $"examiner_{Guid.NewGuid():N}";
        var register = new RegisterDto {
            FirstName = "Иван",
            LastName = "Тестов",
            Login = login,
            Password = "TestPass123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/examiners/register", register);
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await _client.PostAsJsonAsync("/api/examiners/login", new LoginDto {
            Login = login,
            Password = "TestPass123!"
        });
        loginResponse.EnsureSuccessStatusCode();
        Assert.False(string.IsNullOrWhiteSpace(await loginResponse.Content.ReadAsStringAsync()));
    }

    [Fact]
    public async Task Scenes_Page_WithoutJwt_ReturnsUnauthorized() {
        var response = await _client.GetAsync("/api/scenes/page?page=1&limit=10");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Scenes_Meta_WithoutMac_ReturnsUnauthorized() {
        var response = await _client.GetAsync("/api/scenes/meta");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task VrHeadsets_Create_RequiresJwt() {
        var response = await _client.PostAsJsonAsync("/api/vr-headsets", new {
            name = "АРМ",
            macAddress = "AABBCCDDEEFF",
            sceneId = 1L
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<string> RegisterAndGetTokenAsync() {
        var login = $"examiner_{Guid.NewGuid():N}";
        var response = await _client.PostAsJsonAsync("/api/examiners/register", new RegisterDto {
            FirstName = "Пётр",
            LastName = "Проверочный",
            Login = login,
            Password = "SecurePass1!"
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadAsStringAsync()).Trim('"');
    }

    private static ByteArrayContent CreateMinimalJpegContent() {
        var jpeg = Convert.FromBase64String(
            "/9j/4AAQSkZJRgABAQEASABIAAD/2wBDAP//////////////////////////////////////////////////////////////////////////////////////2wBDAf//////////////////////////////////////////////////////////////////////////////////////wAARCAABAAEDAREAAhEBAxEB/8QAFAABAAAAAAAAAAAAAAAAAAAACf/EABQBAQAAAAAAAAAAAAAAAAAAAAD/xAAUAQEAAAAAAAAAAAAAAAAAAAAA/8QAFBEBAAAAAAAAAAAAAAAAAAAAAP/aAAwDAQACEQMRAD8AfwD/2Q==");
        var content = new ByteArrayContent(jpeg);
        content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        return content;
    }
}
