using System.Threading.Tasks;
using CSService.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CSService.Blazor.Layout;

public partial class NavMenu
{
    [Inject]
    public AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject]
    public ICustomLocalStorageService LocalStorageService { get; set; }

    private bool collapseNavMenu = true;

    private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    private async Task LogoutAsync() {
        await LocalStorageService.RemoveToken();
        await AuthStateProvider.GetAuthenticationStateAsync();
    }

    private void ToggleNavMenu() => collapseNavMenu = !collapseNavMenu;
}
