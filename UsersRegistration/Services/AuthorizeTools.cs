using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UsersRegistration.Db;
using UsersRegistration.Models;
using UsersRegistration.Repo;

namespace UsersRegistration.Services;

public class AuthorizeTools
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthorizeTools(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    public object GenerateToken(UserModel userModel)
    {
        var securityKey = new RsaSecurityKey(RsaTools.GetPrivateKey());
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature);

        var userId = _userRepository.GetUserId(userModel);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, userModel.Email),
            new Claim(ClaimTypes.Role, userModel.Role.ToString())
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public UserRole RoleIdToRole(RoleId roleId)
    {
        if (roleId == RoleId.Admin) return UserRole.Administrator;

        return UserRole.User;
    }
}