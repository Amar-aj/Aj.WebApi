using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("auth")]
[ApiController]
public class AuthController(IAccountService _service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        return Ok(await _service.LoginAsync(request, CancellationToken.None));
    }





    // List of predefined login credentials (replace with database or other authentication mechanism)
    //private List<LoginData> loginDataList = new List<LoginData>
    //{
    //    new LoginData("admin", "pass", "Admin User"),
    //    new LoginData("abc", "cc", "Manager"),
    //    new LoginData("raja", "rr", "Client")
    //};


}
