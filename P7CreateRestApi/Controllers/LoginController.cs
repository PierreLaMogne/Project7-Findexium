using Microsoft.AspNetCore.Mvc;
using FindexiumAPI.Models;

namespace FindexiumAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {             
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //TODO: implement the UserManager from Identity to validate User and return a security token.
            return Unauthorized();
        }            
    }
}