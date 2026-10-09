using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Services.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private const int TokenLifetimeHours = 3;

        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtSecurityToken CreateToken(CustomUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            return new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddHours(TokenLifetimeHours),
                claims: claims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
        }
    }
}
