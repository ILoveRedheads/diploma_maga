using System.Threading.Tasks;
using CSService.Blazor.Components;
using CSService.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CSService.Blazor.Pages.Scenes;

public partial class AddPhotoModal
{
    [Inject]
    public ISceneService SceneService { get; set; }

    [Inject]
    public ITransmitter Transmitter { get; set; }

    [Parameter]
    public EventCallback OnPhotoAdded { get; set; }

    private ModalWindow _modalWindow;
    private IBrowserFile _file;
    private long _sceneId;
    private bool _isLoadingFinish = true;

    public void OpenModal(long sceneId) {
        _sceneId = sceneId;
        _file = null;
        _isLoadingFinish = true;
        _modalWindow.Open();
    }

    private void LoadFile(InputFileChangeEventArgs e) {
        _file = e.File;
    }

    private async Task AddPhotoAsync() {
        if (_file == null) {
            Transmitter.ShowMessage("Выберите файл!");
            return;
        }

        _isLoadingFinish = false;
        StateHasChanged();

        await SceneService.AddPhotoToScene(_sceneId, _file);
        await OnPhotoAdded.InvokeAsync();

        _modalWindow.Close();
        Transmitter.ShowMessage("Фотография успешно добавлена!");
    }
}
