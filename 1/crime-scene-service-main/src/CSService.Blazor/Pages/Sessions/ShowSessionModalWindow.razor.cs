using System.Threading.Tasks;
using CSService.Blazor.Components;
using CSService.Blazor.Services;
using CSService.Contracts.Sessions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSService.Blazor.Pages.Sessions;

public partial class ShowSessionModalWindow
{
    [Inject]
    public ISessionService SessionService { get; set; }

    [Inject]
    public IJSRuntime JS { get; set; }

    [Parameter]
    public EventCallback OnDelete { get; set; }

    private ModalWindow _modalWindow;
    private SessionDto _session;

    protected override async Task OnInitializedAsync() {
        await base.OnInitializedAsync();
        _ = await JS.InvokeAsync<IJSObjectReference>("import", "./js/app.js");
    }

    public async Task OpenModalWindow(long id) {
        _session = await SessionService.Get(id);
        _modalWindow.Open();
    }

    private async Task Delete() {
        await SessionService.Delete(_session.Id);
        await OnDelete.InvokeAsync();
        _modalWindow.Close();
    }

    private async ValueTask DownloadFile() =>
        await JS.InvokeVoidAsync(
            "downloadFile",
            new DotNetStreamReference(await SessionService.GetProtocol(_session.Id)));
}
