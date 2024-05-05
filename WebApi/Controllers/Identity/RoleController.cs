using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController(IRoleServices _service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostAsync(RoleAddRequest request)
            => ApiResponseHandler.Handle(await _service.CreateAsync(request, cancellationToken: CancellationToken.None));

        [HttpPut("{role_id:long}")]
        public async Task<IActionResult> PutAsync(long role_id, RoleEditRequest request)
            => ApiResponseHandler.Handle(await _service.UpdateAsync(role_id, request, cancellationToken: CancellationToken.None));

        [HttpDelete("{role_id:long}")]
        public async Task<IActionResult> DeleteAsync(long role_id)
             => ApiResponseHandler.Handle(await _service.DeleteAsync(role_id, cancellationToken: CancellationToken.None));

        [HttpGet("{role_id:long}")]
        public async Task<IActionResult> GetAsync(long role_id)
        {
            return Ok(await _service.GetAsync(role_id, cancellationToken: CancellationToken.None));
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync(int page_number, int page_size)
        {
            return Ok(await _service.GetAsync(page_number, page_size, cancellationToken: CancellationToken.None));
        }
    }
}
