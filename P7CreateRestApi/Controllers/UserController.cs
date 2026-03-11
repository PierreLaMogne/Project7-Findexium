using FindexiumAPI.Common;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindexiumAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        public UserController(IUserRepository userRepository)
        {
            _repository = userRepository;
        }

        // GET: api/User
        [HttpGet]
        [Authorize(Policy = "Users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _repository.GetAllUsersAsync();
            if (!users.Any())
                return NotFound("No User found.");

            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        [Authorize(Policy = "Users")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> PostUser(CreateUserDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repository.CreateUserAsync(request);
            if (!result.IsSuccess)
            {
                if (result.Code == "409")
                    return Conflict(result.ErrorMessage);
                else
                    return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutUser(string id, UserDto request)
        {
            if (id != request.Id)
                return BadRequest("The Id focused and the Id mentioned are different.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repository.UpdateUserAsync(id, request);
            if (!result.IsSuccess)
            {
                if (result.Code == "404")
                    return NotFound(result.ErrorMessage);
                else if (result.Code == "409")
                    return Conflict(result.ErrorMessage);
                else
                    return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _repository.DeleteUserAsync(id);
            if (!result.IsSuccess)
            { if (result.Code == "404")
                    return NotFound(result.ErrorMessage);
                else
                    return BadRequest(result.ErrorMessage);
            }

            return Ok("The User mentioned has been deleted.");
        }
    }
}