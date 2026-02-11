using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace CSService.Blazor.Services.Impl;

public sealed class CustomLocalStorageService(ILocalStorageService localStorageService) : ICustomLocalStorageService
{
    private const string AUTH_TOKEN = "auth_token";

    public ValueTask<string> GetToken() => localStorageService.GetItemAsStringAsync(AUTH_TOKEN);

    public ValueTask RemoveToken() => localStorageService.RemoveItemAsync(AUTH_TOKEN);

    public ValueTask SetToken(string token) => localStorageService.SetItemAsStringAsync(AUTH_TOKEN, token);
}
