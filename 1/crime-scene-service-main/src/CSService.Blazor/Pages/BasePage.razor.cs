using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CSService.Blazor.Pages;

public partial class BasePage
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected override Task OnInitializedAsync() => AuthenticationStateProvider.GetAuthenticationStateAsync();
}
