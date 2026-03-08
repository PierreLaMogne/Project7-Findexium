using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindexiumAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingRepository _repository;

        public RatingController(IRatingRepository ratingRepository)
        {
            _repository = ratingRepository;
        }

        [HttpGet]
        [Authorize(Policy = "Users")]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatings()
        {
            var ratings = await _repository.GetAllAsync();
            return Ok(ratings);
        }

        [HttpGet]
        [Authorize(Policy = "Users")]
        [Route("{id}")]
        public async Task<ActionResult<RatingDto>> GetRating(int id)
        {
            var rating = await _repository.GetByIdAsync(id);
            if (rating == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(rating);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RatingDto>> PostRating(RatingDto rating)
        {
            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var createdRating = await _repository.AddAsync(rating);
            return CreatedAtAction(nameof(GetRating), new { id = createdRating.Id }, createdRating);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<IActionResult> PutRating(int id, RatingDto rating)
        {
            if (id != rating.Id)
                return BadRequest("The Id focused and the Id mentioned are different.");

            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var updated = await _repository.UpdateAsync(id, rating);
            if (!updated)
                return NotFound("The Id mentioned does not exist.");

            return NoContent();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound("The Id mentioned does not exist.");

            return NoContent();
        }
    }
}
