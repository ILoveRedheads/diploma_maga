using System.Linq;
using System.Threading.Tasks;
using CSService.Blazor.Components;
using CSService.Blazor.Services;
using CSService.Contracts;
using CSService.Contracts.Scenes;
using CSService.Contracts.VrHeadsets;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CSService.Blazor.Pages.VrHeadsets;

public partial class VrHeadsetModal
{
    [Inject]
    public IVrHeadsetService VrHeadsetService { get; set; }

    [Inject]
    public ISceneService SceneService { get; set; }

    [Inject]
    public ITransmitter Transmitter { get; set; }

    [Parameter]
    public EventCallback OnCreate { get; set; }

    public long? id;
    private string _title;
    private string _addLabel;
    private ModalWindow _modalWindow;
    private EditContext _editContext;
    private VrHeadsetSetDto _vrHeadsetSetDto;
    private PageQuery _pageQuery = new();
    private PageResult<SceneDto> _scenes = new() { Data = Enumerable.Empty<SceneDto>() };

    public async Task OpenModalWindowAsync() {
        _vrHeadsetSetDto = new() {
            Name = string.Empty,
            MacAddress = string.Empty
        };

        _editContext = new(_vrHeadsetSetDto);
        _title = "Создание записи о VR гарнитуре";
        _addLabel = "Создать";

        await GetScenesAsync();

        _modalWindow.Open();
    }

    public async Task OpenModalWindowAsync(VrHeadsetDto vrHeadsetDto) {
        id = vrHeadsetDto.Id;
        _vrHeadsetSetDto = vrHeadsetDto;
        _editContext = new(_vrHeadsetSetDto);
        _title = "Обновление записи о VR гарнитуре";
        _addLabel = "Обновить";

        await GetScenesAsync();

        _modalWindow.Open();
    }

    private async Task GetScenesAsync() {
        _scenes = await SceneService.GetPage(_pageQuery);
        StateHasChanged();
    }

    private async Task SubmitAsync() {
        if (_editContext.Validate()) {
            if (id.HasValue) {
                await VrHeadsetService.Update(id.Value, _vrHeadsetSetDto);
                Transmitter.ShowMessage("Запись о VR гарнитуре успешно обновлена!");
            } else {
                await VrHeadsetService.Create(_vrHeadsetSetDto);
                Transmitter.ShowMessage("Запись о VR гарнитуре успешно создана!");
            }

            await OnCreate.InvokeAsync();

            id = null;
            _modalWindow.Close();
        }
    }
}
