using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Components;

public partial class ModalWindow
{
    [Parameter]
    public string ModalId { get; set; }

    [Parameter]
    public string ModalBodyClass { get; set; }

    [Parameter]
    public RenderFragment Header { get; set; }

    [Parameter]
    public RenderFragment Body { get; set; }

    [Parameter]
    public string WindowWidth { get; set; }

    [Parameter]
    public string WindowHeight { get; set; }

    [Parameter]
    public string OverflowY { get; set; }

    [Parameter]
    public string AlignSelf { get; set; }

    private bool _showModal = false;

    protected override void OnParametersSet() {
        base.OnParametersSet();
        WindowHeight ??= "fit-content";
        OverflowY ??= "auto";
        AlignSelf ??= "auto";
    }

    public void Open() {
        _showModal = true;
        StateHasChanged();
    }

    public void Close() {
        _showModal = false;
        StateHasChanged();
    }

}
