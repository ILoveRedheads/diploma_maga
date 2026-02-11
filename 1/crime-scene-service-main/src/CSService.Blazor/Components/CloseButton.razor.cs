using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Components;

public partial class CloseButton
{
    [Parameter]
    public EventCallback OnClick { get; set; }
}
