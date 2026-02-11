using System.Threading.Tasks;
using CSService.Contracts;
using CSService.Contracts.Scenes;
using Microsoft.AspNetCore.Components.Forms;

namespace CSService.Blazor.Services;

public interface ISceneService
{
    Task CreateScene(string name, IBrowserFile file);

    Task<PageResult<SceneDto>> GetPage(PageQuery pageQuery);
}
