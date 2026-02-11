using System.Collections.Generic;
using System.Threading.Tasks;
using CSService.Contracts.VrHeadsets;

namespace CSService.Blazor.Services.Impl;

public sealed class VrHeadsetService(IAuthHttpRepository repository) : IVrHeadsetService
{
    private const string VR_HEADSETS = "/api/vr-headsets";
    private const string VR_HEADSETS_SET_SCENE = "/api/vr-headsets/{0}";

    public Task Create(VrHeadsetSetDto setDto) => repository.PostRequest(VR_HEADSETS, setDto);

    public Task<IEnumerable<VrHeadsetDto>> GetList() =>
        repository.GetRequest<IEnumerable<VrHeadsetDto>>(VR_HEADSETS);

    public Task Update(long id, VrHeadsetSetDto setDto) =>
        repository.PutRequest(string.Format(VR_HEADSETS_SET_SCENE, id), setDto);
}
