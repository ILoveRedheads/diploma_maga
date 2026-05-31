using System.Linq;
using System.Threading.Tasks;
using CSService.Blazor.Services;
using CSService.Contracts;
using CSService.Contracts.Scenes;
using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Pages.Scenes;

public partial class ScenesPage
{
    [Inject]
    public ISceneService SceneService { get; set; }

    private bool _isLoadingFinish = true;
    private PageQuery _pageQuery = new();
    private PageResult<SceneDto> _scenes = new() { Data = Enumerable.Empty<SceneDto>() };
    private CreateSceneModal _createSceneModal;
    private AddPhotoModal _addPhotoModal;

    protected override async Task OnInitializedAsync() {
        await base.OnInitializedAsync();
        await GetPageAsync();
    }

    private async Task GetPageAsync() {
        _isLoadingFinish = false;
        _scenes = await SceneService.GetPage(_pageQuery);
        _isLoadingFinish = true;

        StateHasChanged();
    }

    private void OpenAddPhotoModal(long sceneId) {
        _addPhotoModal.OpenModal(sceneId);
    }
}
