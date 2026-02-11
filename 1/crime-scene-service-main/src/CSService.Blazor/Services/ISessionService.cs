using System.IO;
using System.Threading.Tasks;
using CSService.Contracts;
using CSService.Contracts.Sessions;

namespace CSService.Blazor.Services;

public interface ISessionService
{
    Task<PageResult<SessionShortDto>> GetPage(PageQuery pageQuery);

    Task<SessionDto> Get(long id);

    Task Delete(long id);

    Task<Stream> GetProtocol(long id);
}
