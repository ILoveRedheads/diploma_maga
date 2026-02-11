using System.Collections.Generic;
using System.Threading.Tasks;
using CSService.Contracts.VrHeadsets;

namespace CSService.Blazor.Services;

public interface IVrHeadsetService
{
    Task Create(VrHeadsetSetDto setDto);

    Task<IEnumerable<VrHeadsetDto>> GetList();

    Task Update(long id, VrHeadsetSetDto setDto);
}
