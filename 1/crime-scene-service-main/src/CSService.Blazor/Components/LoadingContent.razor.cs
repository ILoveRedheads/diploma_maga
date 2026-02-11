using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Components;

public partial class LoadingContent
{
    [Parameter]
    [EditorRequired]
    public bool IsLoadingFinish { get; set; }

    [Parameter]
    [EditorRequired]
    public RenderFragment ChildContent { get; set; }
}
