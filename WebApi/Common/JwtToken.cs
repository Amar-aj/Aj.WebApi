using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Common
{
    public static class JwtToken
    {
        public static string GenerateToken(string username, string role)
        {
            // Set token issuance and expiration times
            DateTime issuedAt = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(10);  // Adjust expiration time as needed

            // Create claims identity with user information
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, username),
        new Claim(ClaimTypes.Name, username),
        new Claim("role", role)
    });

            // **Security Warning:** Replace with a secure random key generation mechanism and proper key storage
            const string secret_key = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // **REPLACE THIS**
            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secret_key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            const string issuer = "http://localhost:50191";
            const string audience = "http://localhost:50192";

            var tokenHandler = new JwtSecurityTokenHandler();

            // Create the JWT security token
            var token = tokenHandler.CreateJwtSecurityToken(
                issuer: issuer,
                audience: audience,
                subject: claimsIdentity,
                notBefore: issuedAt,
                expires: expires,
                signingCredentials: signingCredentials);

            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

    }
}
