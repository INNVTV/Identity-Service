using Core.Infrastructure.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Core.Application.Authentication.Helpers
{
    public static class GenerateJwtToken
    {
        public static string GenerateJwtTokenString(ICoreConfiguration coreConfiguration, string userId, string userName, string nameKey, string email, string firstName, string lastName, List<string> roles)
        {
            var rsaProvider = new RSACryptoServiceProvider();

            // Note: Requires the RsaCryptoExtensions.cs class in 'Helpers' folder (ToXMLString(true/flase) does not work in .Net Core so we have an extention method that parses pub/priv without boolean flag)
            rsaProvider.FromXmlRsaString(coreConfiguration.JSONWebTokens.PrivateKeyXmlString);
            var rsaKey = new RsaSecurityKey(rsaProvider);

            var signingCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);

            List<Claim> claims = new List<Claim>()
                {
                    new Claim("id", userId),
                    new Claim("userName", userName),
                    new Claim("nameKey", nameKey),
                    new Claim("email", email),
                    new Claim("emailAddress", email),
                    new Claim("firstName", firstName),
                    new Claim("lastName", lastName),
                    new Claim("fullName", $"{firstName} {lastName}"),
                };

            // Add roles to the claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var handler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = handler.CreateJwtSecurityToken(
                coreConfiguration.JSONWebTokens.Issuer,
                coreConfiguration.JSONWebTokens.Audience,
                new ClaimsIdentity(claims),
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(coreConfiguration.JSONWebTokens.TokenExpirationHours),
                DateTime.UtcNow,
                signingCredentials
                );

            var jwtTokenString = handler.WriteToken(jwtSecurityToken);

            return jwtTokenString;
        }
    }
}
