using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers.Identity;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountService _service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostAsync(UserAddEditRequest request)
    {
        return Ok(await _service.CreateAsync(request, cancellationToken: CancellationToken.None));
    }
    [HttpPut("{user_id:long}")]
    public async Task<IActionResult> PutAsync(long user_id, UserAddEditRequest request)
    {
        return Ok(await _service.UpdateAsync(user_id, request, cancellationToken: CancellationToken.None));
    }
    [HttpDelete("{user_id:long}")]
    public async Task<IActionResult> DeleteAsync(long user_id)
    {
        return Ok(await _service.DeleteAsync(user_id, cancellationToken: CancellationToken.None));
    }
    [HttpGet]
    public async Task<IActionResult> GetAsync(int page_number, int page_size)
    {
        return Ok(await _service.GetAsync(page_number, page_size, cancellationToken: CancellationToken.None));
    }
}
