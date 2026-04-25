using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FoodDeliveryAPI.Application.Auth
{
    public interface ITokenService
    {
        JwtSecurityToken GenerateToken(IEnumerable<Claim> claims, IConfiguration _config);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration _config);

       
    }
}
