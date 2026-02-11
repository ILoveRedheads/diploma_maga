using System.Threading.Tasks;
using CSService.Contracts.Examiners;

namespace CSService.Blazor.Services;

public interface IExaminerService
{
    Task<string> Login(LoginDto loginDto);

    Task<string> Register(RegisterDto registerDto);
}
