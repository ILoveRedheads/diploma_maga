using System.Threading.Tasks;
using CSService.API.Attributes;
using CSService.Common.Authorization;
using CSService.Contracts;
using CSService.Contracts.Sessions;
using CSService.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[KebabCaseNaming]
public sealed class SessionsController(ISessionQueries queries) : ControllerBase
{
    [HttpPost]
    [VrHeadsetAuthorization]
    public Task<long> Start([FromBody] SessionSetDto dto) => queries.CreateAsync(dto);

    [HttpGet("page")]
    [Authorize(Roles = Role.Examiner)]
    public Task<PageResult<SessionShortDto>> GetPage([FromQuery] PageQuery query) => queries.GetPageAsync(query);

    [HttpPost("{id:long}/photos")]
    [VrHeadsetAuthorization]
    public Task CreatePhotoComment([FromRoute] long id, IFormFile ScreenshotFile, IFormFile AudioFile) =>
        queries.CreatePhotoAsync(id, ScreenshotFile, AudioFile);

    [HttpGet("photos/{id:long}/screenshot")]
    public async Task<IActionResult> GetPhotoScreenshot([FromRoute] long id, [FromQuery] string hash) {
        var result = await queries.GetPhotoScreenshotAsync(new() { Id = id, Hash = hash });
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpGet("photos/{id:long}/audio")]
    public async Task<IActionResult> GetPhotoAudio([FromRoute] long id, [FromQuery] string hash) {
        var result = await queries.GetPhotoAudioAsync(new() { Id = id, Hash = hash });
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpPost("{id:long}/comments")]
    [VrHeadsetAuthorization]
    public Task CreateComment([FromRoute] long id, IFormFile AudioFile) => queries.CreateCommentAsync(id, AudioFile);

    [HttpGet("comments/{id:long}/audio")]
    public async Task<IActionResult> GetCommentAudio([FromRoute] long id, [FromQuery] string hash) {
        var result = await queries.GetCommentAudioAsync(new() { Id = id, Hash = hash });
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpGet("{id:long}/protocol")]
    [Authorize(Roles = Role.Examiner)]
    public async Task<IActionResult> GetProtocol([FromRoute] long id) {
        var result = await queries.GetProtocolAsync(id);
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = Role.Examiner)]
    public Task<SessionDto> Get([FromRoute] long id) => queries.GetAsync(id);

    [HttpDelete("{id:long}")]
    [Authorize(Roles = Role.Examiner)]
    public Task Delete([FromRoute] long id) => queries.DeleteAsync(id);
}
