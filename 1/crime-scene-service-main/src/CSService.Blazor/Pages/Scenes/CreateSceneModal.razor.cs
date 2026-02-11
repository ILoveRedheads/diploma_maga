using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CSService.Blazor.Components;
using CSService.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CSService.Blazor.Pages.Scenes;

public partial class CreateSceneModal
{
    [Inject]
    public ISceneService SceneService { get; set; }

    [Inject]
    public ITransmitter Transmitter { get; set; }

    [Parameter]
    public EventCallback OnCreate {  get; set; }

    [Required]
    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; }

    private ModalWindow _modalWindow;
    private EditContext _editContext;
    private IBrowserFile _inputFile;
    private bool _isLoadingFinish = true;

    public void OpenModalWindow() {
        _isLoadingFinish = true;
        _editContext = new(this);
        _modalWindow.Open();
    }

    private async Task SubmitAsync() {
        if (_editContext.Validate()) {
            _isLoadingFinish = false;
            StateHasChanged();

            await SceneService.CreateScene(Name, _inputFile);
            await OnCreate.InvokeAsync();

            _modalWindow.Close();

            Transmitter.ShowMessage("Сцена успешна создана!");
        }
    }
}
