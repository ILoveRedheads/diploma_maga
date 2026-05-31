using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CSService.Contracts;
using CSService.Contracts.Scenes;
using Microsoft.AspNetCore.Components.Forms;

namespace CSService.Blazor.Services.Impl;

public sealed class SceneService(IAuthHttpRepository repository) : ISceneService
{
    private const string SCENES = "api/scenes";
    private const string SCENES_PAGE = "api/scenes/page";

    public async Task CreateScene(string name, IBrowserFile file) {
        using var multipartFormContent = new MultipartFormDataContent();
        using var fileStream = file.OpenReadStream(1024 * 1024 * 64);
        multipartFormContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        multipartFormContent.Add(streamContent, "formFile", file.Name);
        multipartFormContent.Add(new StringContent(name), "name");

        await repository.PostRequest(SCENES, multipartFormContent);
    }

    public async Task AddPhotoToScene(long sceneId, IBrowserFile file) {
        using var multipartFormContent = new MultipartFormDataContent();
        using var fileStream = file.OpenReadStream(1024 * 1024 * 64);
        multipartFormContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        multipartFormContent.Add(streamContent, "formFile", file.Name);

        await repository.PostRequest($"{SCENES}/{sceneId}/photos", multipartFormContent);
    }

    public Task<PageResult<SceneDto>> GetPage(PageQuery pageQuery) {
        var query = new Dictionary<string, string>() {
            ["page"] = pageQuery.Page.ToString(),
            ["limit"] = pageQuery.Limit.ToString()
        };

        return repository.GetRequest<PageResult<SceneDto>>(SCENES_PAGE, query);
    }
}
