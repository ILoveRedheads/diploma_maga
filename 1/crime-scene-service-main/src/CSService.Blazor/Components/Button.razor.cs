using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Components;

public partial class Button
{
    [Parameter]
    [EditorRequired]
    public string Label { get; set; }

    [Parameter]
    [EditorRequired]
    public EventCallback OnClick { get; set; }
}
