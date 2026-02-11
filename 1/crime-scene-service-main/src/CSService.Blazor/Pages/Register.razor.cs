using System.Threading.Tasks;
using CSService.Blazor.Services;
using CSService.Contracts.Examiners;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CSService.Blazor.Pages;

public partial class Register
{
    [Inject]
    public IExaminerService ExaminerService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject]
    public ICustomNavigationManager NavigationManager { get; set; }

    [Inject]
    public ICustomLocalStorageService LocalStorageService { get; set; }

    private bool _showPassword;
    private RegisterDto _registerDto = new RegisterDto() {
        FirstName = string.Empty,
        LastName = string.Empty,
        Login = string.Empty,
        Password = string.Empty
    };

    private async Task RegisterAsync() {
        var token = await ExaminerService.Register(_registerDto);
        await LocalStorageService.SetToken(token);
        await AuthStateProvider.GetAuthenticationStateAsync();

        NavigationManager.NavigateToHomePage();
    }

    public void TogglePasswordVisibility() => _showPassword = !_showPassword;
}