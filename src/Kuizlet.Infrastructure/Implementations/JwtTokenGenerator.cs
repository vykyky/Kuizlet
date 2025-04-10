using Kuizlet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Kuizlet.Application.Interfaces.Repositories;


namespace Kuizlet.Infrastructure.Implementations
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly SymmetricSecurityKey _secretKey;
        private static readonly long _tokenExpirationMs = 24 * 60 * 60 * 1000;

        public JwtTokenGenerator(IConfiguration config)
        {
            var secret = config["Jwt:Secret"]
                ?? throw new ArgumentNullException("JWT Secret is missing in configuration");
            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public string GenerateToken(string login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, login) // Аналог subject в Java
            }),
                Expires = DateTime.UtcNow.AddMilliseconds(_tokenExpirationMs),
                SigningCredentials = new SigningCredentials(
                    _secretKey,
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string ExtractLogin(string token)
        {
            var claims = ExtractAllClaims(token);
            return claims.FindFirst(ClaimTypes.Name)?.Value;
        }

        public bool ValidateToken(string token, string expectedUsername)
        {
            try
            {
                var username = ExtractLogin(token);
                return username == expectedUsername && !IsTokenExpired(token);
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"Invalid token: {ex.Message}");
                return false;
            }
        }

        private bool IsTokenExpired(string token)
        {
            var claims = ExtractAllClaims(token);
            var exp = claims.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
            if (exp == null) return true;

            var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).DateTime;
            return expDate <= DateTime.UtcNow;
        }

        private ClaimsPrincipal ExtractAllClaims(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _secretKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
    }
}
