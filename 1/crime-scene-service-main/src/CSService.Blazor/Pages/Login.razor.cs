using System.Threading.Tasks;
using CSService.Blazor.Services;
using CSService.Contracts.Examiners;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CSService.Blazor.Pages;

public partial class Login
{
    [Inject]
    public IExaminerService ExaminerService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject]
    public ICustomNavigationManager NavigationManager { get; set; }

    [Inject]
    public ICustomLocalStorageService LocalStorageService { get; set; }

    private readonly LoginDto _loginDto = new();
    private bool _showPassword;

    private async Task LoginAsync() {
        var token = await ExaminerService.Login(_loginDto);
        await LocalStorageService.SetToken(token);
        await AuthStateProvider.GetAuthenticationStateAsync();

        NavigationManager.NavigateToHomePage();
    }

    private void TogglePasswordVisibility() => _showPassword = !_showPassword;
}
