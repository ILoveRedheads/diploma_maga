using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CSService.Blazor.Enums;
using CSService.Contracts;

namespace CSService.Blazor.Services.Impl;

public class HttpRepository : IHttpRepository
{
    private readonly HttpClient _httpClient;
    private readonly ITransmitter _transmitter;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpRepository(HttpClient httpClient, ITransmitter transmitter) {
        _httpClient = httpClient;
        _transmitter = transmitter;

        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public virtual async Task DeleteRequest(string route) {
        var response = await _httpClient.DeleteAsync(route);
        await EnsureSuccessAsync(response);
    }

    public virtual async Task<Stream> GetFileRequest(string route, Dictionary<string, string> queryParams = null) {
        var query = GetQuery(queryParams);
        var response = await _httpClient.GetAsync(route + query);

        await EnsureSuccessAsync(response);

        return await response.Content.ReadAsStreamAsync();
    }

    public virtual async Task<T> GetRequest<T>(string route, Dictionary<string, string> queryParams = null) {
        var query = GetQuery(queryParams);
        var response = await _httpClient.GetAsync(route + query);

        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    public virtual async Task PatchRequest(string route, object body) {
        var response = await _httpClient.PatchAsync(route, JsonContent.Create(body, options: _jsonOptions));
        await EnsureSuccessAsync(response);
    }

    public virtual async Task PostRequest(string route, object body) {
        var response = await _httpClient.PostAsJsonAsync(route, body, _jsonOptions);
        await EnsureSuccessAsync(response);
    }

    public virtual async Task PostRequest(string route, HttpContent body) {
        var response = await _httpClient.PostAsync(route, body);
        await EnsureSuccessAsync(response);
    }

    public virtual async Task<TResponse> PostRequest<TResponse>(string route, object body) {
        var response = await _httpClient.PostAsJsonAsync(route, body, _jsonOptions);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
    }

    public virtual async Task<TResponse> PostRequest<TResponse>(string route, HttpContent body) {
        var response = await _httpClient.PostAsync(route, body);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
    }

    public virtual async Task<string> PostRequestRawResult(string route, object body) {
        var response = await _httpClient.PostAsJsonAsync(route, body, _jsonOptions);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadAsStringAsync();
    }

    public virtual async Task PutRequest(string route, object body) {
        var response = await _httpClient.PutAsJsonAsync(route, body, _jsonOptions);
        await EnsureSuccessAsync(response);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response) {
        try {
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException) {
            var detail = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            _transmitter.ShowMessage(
                string.Format("{0} : {1}", (int)response.StatusCode, detail.Title),
                LevelType.Error);

            throw;
        }
    }

    private static string GetQuery(Dictionary<string, string> queryParams) {
        if (queryParams is null || queryParams.Count == 0) return null;

        return '?' + string.Join('&', queryParams.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
    }
}
