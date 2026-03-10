using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindexiumAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurvePointController : ControllerBase
    {
        private readonly ICurvePointRepository _repository;

        public CurvePointController(ICurvePointRepository curvePointRepository)
        {
            _repository = curvePointRepository;
        }

        [HttpGet]
        [Authorize(Policy = "Users")]
        public async Task<ActionResult<IEnumerable<CurvePointDto>>> GetCurvePoints()
        {
            var curvePoints = await _repository.GetAllAsync();
            if (!curvePoints.Any())
                return NotFound("No CurvePoint found.");

            return Ok(curvePoints);
        }

        [HttpGet]
        [Authorize(Policy = "Users")]
        [Route("{id}")]
        public async Task<ActionResult<CurvePointDto>> GetCurvePoint(int id)
        {
            var curvePoint = await _repository.GetByIdAsync(id);
            if (curvePoint == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(curvePoint);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CurvePointDto>> PostCurvePoint(CurvePointDto curvePoint)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCurvePoint = await _repository.AddAsync(curvePoint);
            return CreatedAtAction(nameof(GetCurvePoint), new { id = createdCurvePoint.Id }, createdCurvePoint);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<IActionResult> PutCurvePoint(int id, CurvePointDto curvePoint)
        {
            if (id != curvePoint.Id)
                return BadRequest("The Id focused and the Id mentioned are different.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _repository.UpdateAsync(id, curvePoint);
            if (!updated)
                return NotFound("The Id mentioned does not exist.");

            return Ok("The CurvePoint mentioned has been updated.");
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCurvePoint(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound("The Id mentioned does not exist.");

            return Ok("The CurvePoint mentioned has been deleted.");
        }
    }
}
