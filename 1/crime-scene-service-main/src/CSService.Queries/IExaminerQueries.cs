using System.Threading.Tasks;
using CSService.Contracts.Examiners;

namespace CSService.Queries;

public interface IExaminerQueries
{
    Task<string> RegisterAsync(RegisterDto registerDto);

    Task<string> LoginAsync(LoginDto loginDto);
}
