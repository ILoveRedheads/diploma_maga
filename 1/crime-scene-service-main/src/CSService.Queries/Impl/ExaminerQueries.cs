using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using CSService.Common.Authorization;
using CSService.Common.DataAccess;
using CSService.Contracts.Examiners;
using CSService.Common.DataAccess.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CSService.Queries.Impl;

internal sealed partial class ExaminerQueries(
    ISqlRepository repository,
    IPasswordHashService passwordHashService,
    IJwtSettings jwtSettings) : IExaminerQueries
{
    public async Task<string> RegisterAsync(RegisterDto registerDto) {
        var existedExaminer = await repository.Query<Examiner>(GET_SQL, registerDto);
        if (existedExaminer is not null)
            throw new ArgumentException($"Пользователь стаким логином {registerDto.Login} уже существует!");

        var hashedPassword = passwordHashService.Hash(registerDto.Password);
        var examiner = new Examiner {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Login = registerDto.Login,
            Password = hashedPassword.Hash,
            PasswordSalt = hashedPassword.Salt,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
        };

        await repository.Execute(CREATE_SQL, examiner);
        return GetJwtToken(examiner.Login);
    }

    public async Task<string> LoginAsync(LoginDto loginDto) {
        var examiner = await repository.Query<Examiner>(GET_SQL, new { loginDto.Login })
            ?? throw new ArgumentException($"Пользователь с таким логином {loginDto.Login} не найден!");

        var hashedPassword = new HashedPassword() {
            Hash = examiner.Password,
            Salt = examiner.PasswordSalt
        };

        if (!passwordHashService.Verify(hashedPassword, loginDto.Password))
            throw new ArgumentException("Неверный пароль!");

        return GetJwtToken(examiner.Login);
    }

    private string GetJwtToken(string login) {
        var claims = new List<Claim> { new(ClaimTypes.Role, Role.Examiner), new(ClaimsConst.LOGIN, login) };
        var jwt = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(jwtSettings.JwtLifeTime),
            signingCredentials: new SigningCredentials(jwtSettings.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
