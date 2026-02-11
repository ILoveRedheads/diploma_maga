using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace CSService.Blazor.Services.Impl;

public sealed class AuthHttpRepository(
    HttpClient httpClient,
    ITransmitter transmitter,
    AuthenticationStateProvider authStateProvider)
    : HttpRepository(httpClient, transmitter), IAuthHttpRepository
{
    public override async Task DeleteRequest(string route) {
        await authStateProvider.GetAuthenticationStateAsync();
        await base.DeleteRequest(route);
    }

    public override async Task<Stream> GetFileRequest(string route, Dictionary<string, string> queryParams = null) {
        await authStateProvider.GetAuthenticationStateAsync();
        return await base.GetFileRequest(route, queryParams);
    }

    public override async Task<T> GetRequest<T>(string route, Dictionary<string, string> queryParams = null) {
        await authStateProvider.GetAuthenticationStateAsync();
        return await base.GetRequest<T>(route, queryParams);
    }

    public override async Task PatchRequest(string route, object body) {
        await authStateProvider.GetAuthenticationStateAsync();
        await base.PatchRequest(route, body);
    }

    public override async Task PostRequest(string route, object body) {
        await authStateProvider.GetAuthenticationStateAsync();
        await base.PostRequest(route, body);
    }

    public override async Task PostRequest(string route, HttpContent body) {
        await authStateProvider.GetAuthenticationStateAsync();
        await base.PostRequest(route, body);
    }

    public override async Task PutRequest(string route, object body) {
        await authStateProvider.GetAuthenticationStateAsync();
        await base.PutRequest(route, body);
    }

    public override async Task<TResponse> PostRequest<TResponse>(string route, object body) {
        await authStateProvider.GetAuthenticationStateAsync();
        return await base.PostRequest<TResponse>(route, body);
    }

    public override async Task<TResponse> PostRequest<TResponse>(string route, HttpContent body) {
        await authStateProvider.GetAuthenticationStateAsync();
        return await base.PostRequest<TResponse>(route, body);
    }

    public override async Task<string> PostRequestRawResult(string route, object body) {
        await authStateProvider.GetAuthenticationStateAsync();
        return await base.PostRequestRawResult(route, body);
    }
}
