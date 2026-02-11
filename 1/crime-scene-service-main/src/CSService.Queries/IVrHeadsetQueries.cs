using System.Collections.Generic;
using System.Threading.Tasks;
using CSService.Contracts.VrHeadsets;

namespace CSService.Queries;

public interface IVrHeadsetQueries
{
    Task CreateAsync(VrHeadsetSetDto setDto);

    Task UpdateAsync(long id, VrHeadsetSetDto setDto);

    Task<IEnumerable<VrHeadsetDto>> GetListAsync();
}
