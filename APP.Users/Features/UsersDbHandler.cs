using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace APP.Users.Features
{
    public class UsersDbHandler : Handler
    {
        protected readonly UsersDb _usersDb;

        public UsersDbHandler(UsersDb usersDb) : base(new CultureInfo("en-US"))
        {
            _usersDb = usersDb;
        }

        protected virtual List<Claim> GetClaims(User user)
        {
            return new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("Id", user.Id.ToString())
            };
        }

        protected virtual string CreateAccessToken(List<Claim> claims, DateTime expiration)
        {
            var signingCredentials = new SigningCredentials(AppSettings.SigningKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: AppSettings.Issuer,
                audience: AppSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiration,
                signingCredentials: signingCredentials
            );
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            return jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
        }

        protected virtual string CreateRefreshToken()
        {
            var bytes = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        protected virtual ClaimsPrincipal GetPrincipal(string accessToken)
        {
            accessToken = accessToken.StartsWith(JwtBearerDefaults.AuthenticationScheme) ?
                accessToken.Remove(0, JwtBearerDefaults.AuthenticationScheme.Length + 1) : accessToken;
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = AppSettings.Issuer,
                ValidAudience = AppSettings.Audience,
                IssuerSigningKey = AppSettings.SigningKey
            };
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = jwtSecurityTokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
            return securityToken is null ? null : principal;
        }
    }
}