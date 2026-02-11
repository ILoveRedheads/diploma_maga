using CSService.Blazor.Components;
using CSService.Blazor.Enums;

namespace CSService.Blazor.Services;

public interface ITransmitter
{
    Toast Toast { set; }

    void ShowMessage(string message, LevelType level = LevelType.Information);
}
