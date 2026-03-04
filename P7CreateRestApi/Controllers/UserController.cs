using Microsoft.AspNetCore.Mvc;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;

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
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _repository.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUser(CreateUserDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var createdUser = await _repository.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, UpdateUserDto request)
        {
            if (id != request.Id)
                return BadRequest("The Id focused and the Id mentioned are different.");

            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var updatedUser = await _repository.UpdateUserAsync(id, request);
            if (updatedUser == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(updatedUser);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var deleted = await _repository.DeleteUserAsync(id);
            if (!deleted)
                return NotFound("The Id mentioned does not exist.");

            return Ok("The User has been deleted.");
        }
    }
}