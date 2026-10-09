using System.IdentityModel.Tokens.Jwt;
using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Services.Auth
{
    /// <summary>
    /// İstifadəçi üçün JWT token yaradır.
    /// </summary>
    public interface IJwtTokenService
    {
        JwtSecurityToken CreateToken(CustomUser user, IEnumerable<string> roles);
    }
}
