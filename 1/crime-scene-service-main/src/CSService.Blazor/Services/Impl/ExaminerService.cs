using System.Threading.Tasks;
using CSService.Contracts.Examiners;

namespace CSService.Blazor.Services.Impl;

public sealed class ExaminerService(IHttpRepository httpRepository) : IExaminerService
{
    private const string LOGIN = "api/examiners/login";
    private const string REGISTER = "api/examiners/register";

    public Task<string> Login(LoginDto loginDto) =>
        httpRepository.PostRequestRawResult(LOGIN, loginDto);

    public Task<string> Register(RegisterDto registerDto) =>
        httpRepository.PostRequestRawResult(REGISTER, registerDto);

}
