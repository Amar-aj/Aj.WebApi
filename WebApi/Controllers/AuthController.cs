using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Models;

namespace WebApi.Controllers;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        //var userData;
        ////var userData = loginDataList.FirstOrDefault(x => x.username == request.username);
        //if (userData == null)
        //{
        //    return BadRequest("Invalid username");
        //}
        //if (userData.password != request.password)
        //{
        //    return BadRequest("Invalid password");
        //}
        //else
        //{
            //var token = GenerateToken(userData.username, userData.tag);
            //return Ok(new { token });
            return Ok("");
        //}
    }


    private string GenerateToken(string username, string tag)
    {
        // Set token issuance and expiration times
        DateTime issuedAt = DateTime.Now;
        DateTime expires = DateTime.Now.AddMinutes(10);  // Adjust expiration time as needed

        // Create claims identity with user information
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
        {
        new Claim(ClaimTypes.Name, username),
        new Claim("user_tag", tag)
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



    // List of predefined login credentials (replace with database or other authentication mechanism)
    //private List<LoginData> loginDataList = new List<LoginData>
    //{
    //    new LoginData("admin", "pass", "Admin User"),
    //    new LoginData("abc", "cc", "Manager"),
    //    new LoginData("raja", "rr", "Client")
    //};


}
