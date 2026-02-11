using System.Threading.Tasks;

namespace CSService.Blazor.Services;

public interface ICustomLocalStorageService
{
    ValueTask SetToken(string token);

    ValueTask<string> GetToken();

    ValueTask RemoveToken();
}
