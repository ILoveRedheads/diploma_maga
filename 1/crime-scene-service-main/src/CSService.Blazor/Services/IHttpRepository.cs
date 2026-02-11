using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSService.Blazor.Services;

public interface IHttpRepository
{
    Task<Stream> GetFileRequest(string route, Dictionary<string, string> queryParams = null);

    Task<T> GetRequest<T>(string route, Dictionary<string, string> queryParams = null);

    Task PostRequest(string route, object body);

    Task PostRequest(string route, HttpContent body);

    Task<TResponse> PostRequest<TResponse>(string route, object body);

    Task<TResponse> PostRequest<TResponse>(string route, HttpContent body);

    Task<string> PostRequestRawResult(string route, object body);

    Task PatchRequest(string route, object body);

    Task PutRequest(string route, object body);

    Task DeleteRequest(string route);
}
