namespace Rokalo.Infrastructure.Security
{
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Rokalo.Application.Contracts.Security;
    using Rokalo.Domain;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    internal sealed class TokenService : ITokenService
    {
        private readonly SecurityAdapterConfigurations settings;

        public TokenService(IOptions<SecurityAdapterConfigurations> settings)
        {
            this.settings = settings.Value;
        }

        public string GenerateJwtToken(User user)
        {
            var issuer = this.settings.JwtTokenConfiguration.Issuer;

            var audience = this.settings.JwtTokenConfiguration.Audience;

            var claims = new List<System.Security.Claims.Claim>()
            {
                new System.Security.Claims.Claim(ClaimTypes.Name, user.Id.ToString()),
                new System.Security.Claims.Claim(ClaimTypes.Email, user.Email),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new System.Security.Claims.Claim(ClaimTypes.Role, "admin-test")
            };

            var notBefore = DateTime.UtcNow;

            var expires = DateTime.UtcNow.AddMinutes(this.settings.JwtTokenConfiguration.ExpiresInMinutes);

            var key = Encoding.UTF8.GetBytes(this.settings.JwtTokenConfiguration.Secret);

            var signInCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                notBefore,
                expires,
                signInCredentials);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }

        public RefreshToken GenerateRefreshToken(User user)
        {
            var token = new RefreshToken(
                Guid.NewGuid(),
                user.Id,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(this.settings.RefreshTokenConfiguration.ValidForDays));

            return token;
        }
    }
}
