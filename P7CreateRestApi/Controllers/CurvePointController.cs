using Microsoft.AspNetCore.Mvc;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;

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
        public async Task<ActionResult<IEnumerable<CurvePointDto>>> GetCurvePoints()
        {
            var curvePoints = await _repository.GetAllAsync();
            return Ok(curvePoints);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CurvePointDto>> GetCurvePoint(int id)
        {
            var curvePoint = await _repository.GetByIdAsync(id);
            if (curvePoint == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(curvePoint);
        }

        [HttpPost]
        public async Task<ActionResult<CurvePointDto>> PostCurvePoint(CurvePointDto curvePoint)
        {
            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var createdCurvePoint = await _repository.AddAsync(curvePoint);
            return CreatedAtAction(nameof(GetCurvePoint), new { id = createdCurvePoint.Id }, createdCurvePoint);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PutCurvePoint(int id, CurvePointDto curvePoint)
        {
            if (id != curvePoint.Id)
                return BadRequest("The Id focused and the Id mentioned are different.");

            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var updated = await _repository.UpdateAsync(id, curvePoint);
            if (!updated)
                return NotFound("The Id mentioned does not exist.");

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCurvePoint(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound("The Id mentioned does not exist.");

            return NoContent();
        }
    }
}
