using System.Threading;
using CSService.Blazor.Enums;
using CSService.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Components;

public partial class Toast
{
    [Inject]
    public ITransmitter Transmitter { get; set; }

    private const string EXCLAMATION_ICON = "bi bi-exclamation-circle toast-error-icon";
    private const string INFO_ICON = "bi bi-info-circle toast-info-icon";
    private const string TOAST_IN = "toast toast-in";
    private const string TOAST_OUT = "toast toast-out";

    private bool _isVisible;
    private LevelType _level;
    private string _message;
    private string _class = TOAST_IN;
    private Timer _timer;
    private string _iconClass => _level switch {
        LevelType.Information => INFO_ICON,
        LevelType.Error => EXCLAMATION_ICON,
        _ => INFO_ICON
    };

    public void Show(string message, LevelType level) {
        if (_isVisible) {
            _isVisible = false;
            StateHasChanged();
        }

        _class = TOAST_IN;
        _message = message;
        _level = level;
        _isVisible = true;

        StateHasChanged();
    }

    public void Hide() {
        if (!_isVisible) return;

        _class = TOAST_OUT;
        _timer?.Dispose();
        _timer = new Timer(_ => {
            _isVisible = false;
            StateHasChanged();
        }, null, 650, 0);

        StateHasChanged();
    }

    protected override void OnInitialized() => Transmitter.Toast = this;
}
