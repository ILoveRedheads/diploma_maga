using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CSService.API.Attributes;
using CSService.Common.Authorization;
using CSService.Contracts;
using CSService.Contracts.Scenes;
using CSService.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[KebabCaseNaming]
public sealed class ScenesController(ISceneQueries queries) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Role.Examiner)]
    public async Task<IActionResult> Create([FromForm][Required][StringLength(128)] string name, IFormFile formFile) {
        if (formFile.Length == 0) return BadRequest();

        long? sceneId;
        using (var stream = formFile.OpenReadStream()) {
            sceneId = await queries.CreateAsync(name, formFile.FileName, stream);
        }

        return Created("api/scene", sceneId);
    }

    [HttpPost("{id:long}/photos")]
    [Authorize(Roles = Role.Examiner)]
    public async Task<IActionResult> AddPhoto([FromRoute] long id, IFormFile formFile) {
        if (formFile.Length == 0) return BadRequest();

        long? photoId;
        using (var stream = formFile.OpenReadStream()) {
            photoId = await queries.AddPhotoToSceneAsync(id, formFile.FileName, stream);
        }

        return Created($"api/scenes/photos/{photoId}", photoId);
    }

    [HttpGet("page")]
    [Authorize(Roles = Role.Examiner)]
    public Task<PageResult<SceneDto>> GetPage([FromQuery] PageQuery context) =>
        queries.GetPageAsync(context);

    [HttpGet]
    [VrHeadsetAuthorization]
    public async Task<IActionResult> GetScene() {
        var result = await queries.GetAsync();
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpGet("meta")]
    [VrHeadsetAuthorization]
    public async Task<SceneMetaDto> GetSceneMeta() => await queries.GetSceneMetaAsync();

    [HttpGet("{id:long}/photos")]
    [Authorize(Roles = Role.Examiner)]
    public async Task<IActionResult> GetScenePhotos([FromRoute] long id) {
        var photos = await queries.GetScenePhotosAsync(id);
        return Ok(photos);
    }

    [HttpGet("photos/{photoId:long}")]
    public async Task<IActionResult> GetScenePhoto([FromRoute] long photoId, [FromQuery] string hash) {
        var result = await queries.GetScenePhotoAsync(photoId);
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpGet("photos/by-index/{photoIndex:int}")]
    [VrHeadsetAuthorization]
    public async Task<IActionResult> GetScenePhotoByIndex([FromRoute] int photoIndex) {
        var result = await queries.GetScenePhotoByIndexAsync(photoIndex);
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpGet("{id:long}/preview")]
    public async Task<IActionResult> GetPreview([FromRoute] long id, [FromQuery] string hash) {
        var result = await queries.GetPreviewAsync(new() { Id = id, Hash = hash });
        return File(result.Content, result.ContentType, result.Name);
    }
}
