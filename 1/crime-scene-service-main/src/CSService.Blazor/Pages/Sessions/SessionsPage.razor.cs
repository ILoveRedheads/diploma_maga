using System.Threading.Tasks;
using CSService.Blazor.Services;
using CSService.Contracts;
using CSService.Contracts.Sessions;
using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Pages.Sessions;

public partial class SessionsPage : BasePage
{
    [Inject]
    public ISessionService SessionService { get; set; }

    [Inject]
    public ITransmitter Transmitter { get; set; }

    private bool _isLoadingFinish = true;
    private PageQuery _pageQuery = new();
    private ShowSessionModalWindow _showSessionModalWindow;
    private PageResult<SessionShortDto> _sessions = new() { Data = [] };

    protected override async Task OnInitializedAsync() {
        await base.OnInitializedAsync();
        await GetPage();
    }

    private async Task GetPage() {
        _isLoadingFinish = false;
        _sessions = await SessionService.GetPage(_pageQuery);
        _isLoadingFinish = true;

        StateHasChanged();
    }
}
