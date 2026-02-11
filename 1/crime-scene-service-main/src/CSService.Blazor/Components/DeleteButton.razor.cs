using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Components;

public partial class DeleteButton
{
    [Parameter]
    public EventCallback OnClick { get; set; }
}
