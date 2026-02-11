using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSService.Blazor.Services;
using CSService.Contracts.VrHeadsets;
using Microsoft.AspNetCore.Components;

namespace CSService.Blazor.Pages.VrHeadsets;

public partial class VrHeadsetsPage
{
    [Inject]
    public IVrHeadsetService VrHeadsetService { get; set; }

    private bool _isLoadingFinish = true;
    private VrHeadsetModal _createVrHeadsetModal;
    private IEnumerable<VrHeadsetDto> _vrHeadsets = Enumerable.Empty<VrHeadsetDto>();

    protected override async Task OnInitializedAsync() {
        await base.OnInitializedAsync();
        await GetListAsync();
    }

    private async Task GetListAsync() {
        _isLoadingFinish = false;
        _vrHeadsets = await VrHeadsetService.GetList();
        _isLoadingFinish = true;

        StateHasChanged();
    }
}
