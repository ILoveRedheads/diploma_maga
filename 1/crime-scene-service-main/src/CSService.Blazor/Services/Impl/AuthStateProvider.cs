using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CSService.Blazor.Services.Impl;

public sealed class AuthStateProvider(
    ICustomNavigationManager navigationManager,
    ICustomLocalStorageService localStorageService,
    HttpClient httpClient) : AuthenticationStateProvider
{
    private static TaskCompletionSource<AuthenticationState> _tcs;
    private static AuthenticationState _state = new(new ClaimsPrincipal());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        var tcs = _tcs;
        if (tcs is not null) return await tcs.Task;

        _tcs = new();
        var token = await localStorageService.GetToken();

        if (!IsValidToken(token)) {
            _state = new(new ClaimsPrincipal());
            navigationManager.NavigateToLoginPage();
        } else if (!_state.User.Claims.Any()) {
            _state = GetAuthState(token);
            SetAuthorizationHeader(token);
        }

        NotifyAuthenticationStateChanged(Task.FromResult(_state));

        _tcs?.SetResult(_state);
        _tcs = null;

        return _state;
    }

    private static bool IsValidToken(string token) {
        if (string.IsNullOrEmpty(token)) return false;

        var jsonWebToken = new JsonWebTokenHandler().ReadJsonWebToken(token);
        var expireClaim = jsonWebToken.Claims.FirstOrDefault(c => c.Type == "exp");

        if (long.TryParse(expireClaim?.Value, out var expireSeconds)) {
            return DateTimeOffset.UtcNow < DateTimeOffset.FromUnixTimeSeconds(expireSeconds);
        }

        return false;
    }

    private static AuthenticationState GetAuthState(string token) {
        var identity = new ClaimsIdentity(new[] { new Claim("auth_token", token) }, JwtConstants.TokenType);
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private void SetAuthorizationHeader(string token) =>
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
}
