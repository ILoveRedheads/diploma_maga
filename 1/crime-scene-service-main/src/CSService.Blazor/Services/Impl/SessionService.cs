using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CSService.Contracts;
using CSService.Contracts.Sessions;

namespace CSService.Blazor.Services.Impl;

public sealed class SessionService(IAuthHttpRepository repository) : ISessionService
{
    private const string SESSION = "api/sessions/{0}";
    private const string SESSIONS_PAGE = "api/sessions/page";
    private const string SESSION_PROTOCOL = "api/sessions/{0}/protocol";

    public Task<SessionDto> Get(long id) => repository.GetRequest<SessionDto>(string.Format(SESSION, id));

    public Task<PageResult<SessionShortDto>> GetPage(PageQuery pageQuery) {
        var query = new Dictionary<string, string>() {
            ["page"] = pageQuery.Page.ToString(),
            ["limit"] = pageQuery.Limit.ToString(),
            ["search"] = pageQuery.Search
        };

        return repository.GetRequest<PageResult<SessionShortDto>>(SESSIONS_PAGE, query);
    }

    public Task Delete(long id) => repository.DeleteRequest(string.Format(SESSION, id));

    public Task<Stream> GetProtocol(long id) => repository.GetFileRequest(string.Format(SESSION_PROTOCOL, id));
}
