using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSService.Contracts;
using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Components;

public partial class Pagination
{
    [Parameter]
    public PageQuery PageQuery { get; set; }

    [Parameter]
    public EventCallback<PageQuery> PageQueryChanged { get; set; }

    [Parameter]
    public int EntitiesCount { get; set; }

    [Parameter]
    public RenderFragment Header { get; set; }

    [Parameter]
    public RenderFragment Body { get; set; }

    [Parameter]
    public RenderFragment Footer { get; set; }

    [Parameter]
    public int Spread { get; set; } = 2;

    [Parameter]
    public bool EnableSearch { get; set; } = false;

    private int _pageCount => (int)Math.Ceiling((double)EntitiesCount / PageQuery.Limit);

    private Timer _timer;

    private async Task OnPageChangeAsync(int page) {
        if (page != PageQuery.Page && page >= 0 && page < _pageCount) {
            PageQuery.Page = page;
            await PageQueryChanged.InvokeAsync();
        }
    }

    private async Task OnLimitChangeAsync() {
        PageQuery.Page = 0;
        await PageQueryChanged.InvokeAsync();
    }

    private IEnumerable<int> GetPageNumbers() {
        var displayStartPage = Math.Max(PageQuery.Page - Spread, 0);
        var displayPageCount = Math.Min(_pageCount - displayStartPage, 2 * Spread + 1);

        if (displayStartPage > 0) {
            var countDifference = (2 * Spread) + 1 - displayPageCount;
            displayStartPage = Math.Max(displayStartPage - countDifference, 0);
            displayPageCount = Math.Min(_pageCount - displayStartPage, 2 * Spread + 1);
        }

        return Enumerable.Range(displayStartPage, displayPageCount);
    }

    private void OnSearchChanged() {
        _timer?.Dispose();
        _timer = new Timer(async _ =>
        {
            PageQuery.Page = 0;

            await PageQueryChanged.InvokeAsync();

            StateHasChanged();
        }, null, 500, 0);
    }
}
