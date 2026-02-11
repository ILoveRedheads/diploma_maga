using System.Collections.Generic;
using System.Threading.Tasks;
using CSService.API.Attributes;
using CSService.Common.Authorization;
using CSService.Contracts.VrHeadsets;
using CSService.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[KebabCaseNaming]
[Authorize(Roles = Role.Examiner)]
public sealed class VrHeadsetsController(IVrHeadsetQueries queries) : ControllerBase
{
    [HttpPost]
    public Task Create([FromBody] VrHeadsetSetDto vrHeadsetSetDto) => queries.CreateAsync(vrHeadsetSetDto);

    [HttpPut("{id:long}")]
    public Task Update([FromRoute] long id, [FromBody] VrHeadsetSetDto setDto) => queries.UpdateAsync(id, setDto);

    [HttpGet]
    public Task<IEnumerable<VrHeadsetDto>> GetList() => queries.GetListAsync();
}
