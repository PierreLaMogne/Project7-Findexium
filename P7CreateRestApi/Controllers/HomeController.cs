using Microsoft.AspNetCore.Mvc;

namespace FindexiumAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet]
        [Route("Admin")]
        public IActionResult Admin()
        {
            return Ok();
        }
    }
}