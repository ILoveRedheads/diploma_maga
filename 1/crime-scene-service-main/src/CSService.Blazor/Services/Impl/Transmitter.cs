using System.Threading;
using CSService.Blazor.Components;
using CSService.Blazor.Enums;

namespace CSService.Blazor.Services.Impl;

public sealed class Transmitter : ITransmitter
{
    public Toast Toast { set => _toast = value; }

    private Toast _toast;
    private Timer _timer;

    public void ShowMessage(string message, LevelType level = LevelType.Information) {
        if (_toast is null) return;

        var dueTime = level switch {
            LevelType.Error => 20000,
            LevelType.Information => 5000,
            LevelType.ActionStatus => 2000,
            _ => 10000
        };

        _toast.Show(message, level);
        _timer?.Dispose();
        _timer = new Timer(_ => _toast.Hide(), null, dueTime, 0);
    }
}
