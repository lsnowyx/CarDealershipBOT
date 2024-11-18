using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ELD888TGBOT
{
    public static class JwtHelper
    {
        static ClaimsPrincipal GetClaimsFromJwt(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(jwtToken))
            {
                var token = handler.ReadJwtToken(jwtToken);
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(token.Claims));

                return claimsPrincipal;
            }
            else
            {
                throw new ArgumentException("Invalid JWT token");
            }
        }
        public static string GetRoleFromJwt(string jwtToken)
        {
            var claimsPrincipal = GetClaimsFromJwt(jwtToken);
            var roleClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "rol");

            return roleClaim?.Value ?? "No role claim found";
        }
    }
}
