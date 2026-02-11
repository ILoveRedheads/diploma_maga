using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Services.Impl;

public sealed class CustomNavigationManager(NavigationManager navigationManager) : ICustomNavigationManager
{
    private const string LOGIN = "/login";
    private const string VR_HEADSETS = "/vr-headsets";

    public void NavigateToHomePage() => navigationManager.NavigateTo(VR_HEADSETS);

    public void NavigateToLoginPage() => navigationManager.NavigateTo(LOGIN);
}
