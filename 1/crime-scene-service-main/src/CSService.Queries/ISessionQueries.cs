using System.Threading.Tasks;
using CSService.Contracts;
using CSService.Contracts.Sessions;
using Microsoft.AspNetCore.Http;

namespace CSService.Queries;

public interface ISessionQueries
{
    Task<long> CreateAsync(SessionSetDto dto);

    Task<PageResult<SessionShortDto>> GetPageAsync(PageQuery query);

    Task CreatePhotoAsync(long id, IFormFile screenshotFile, IFormFile audioFile);

    Task<MediaResult> GetPhotoScreenshotAsync(MediaContext context);

    Task<MediaResult> GetPhotoAudioAsync(MediaContext context);

    Task CreateCommentAsync(long id, IFormFile audioFile);

    Task<MediaResult> GetCommentAudioAsync(MediaContext context);

    Task<MediaResult> GetProtocolAsync(long id);

    Task<SessionDto> GetAsync(long id);

    Task DeleteAsync(long id);
}
