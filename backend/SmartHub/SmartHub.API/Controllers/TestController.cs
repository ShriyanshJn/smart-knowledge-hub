using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("SmartHub.API is protected and reachable");
        }
    }
}
