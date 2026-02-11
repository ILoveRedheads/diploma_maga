using System.Threading.Tasks;
using CSService.API.Attributes;
using CSService.Contracts.Examiners;
using CSService.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CSService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[KebabCaseNaming]
public sealed class ExaminersController(IExaminerQueries queries) : ControllerBase
{
    [HttpPost("register")]
    public Task<string> Register([FromBody] RegisterDto registerDto) => queries.RegisterAsync(registerDto);

    [HttpPost("login")]
    public Task<string> Login([FromBody] LoginDto loginDto) => queries.LoginAsync(loginDto);
}
