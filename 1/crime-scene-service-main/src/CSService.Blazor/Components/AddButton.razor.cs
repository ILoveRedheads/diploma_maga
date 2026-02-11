using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Components;

public partial class AddButton
{
    [Parameter]
    public string Label { get; set; }

    [Parameter]
    public EventCallback OnClick { get; set; }
}
