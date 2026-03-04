using Microsoft.AspNetCore.Mvc;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;

namespace FindexiumAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleNameController : ControllerBase
    {
        private readonly IRuleNameRepository _repository;

        public RuleNameController(IRuleNameRepository ruleNameRepository)
        {
            _repository = ruleNameRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RuleNameDto>>> GetRuleNames()
        {
            var ruleNames = await _repository.GetAllAsync();
            return Ok(ruleNames);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RuleNameDto>> GetRuleName(int id)
        {
            var ruleName = await _repository.GetByIdAsync(id);
            if (ruleName == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(ruleName);
        }

        [HttpPost]
        public async Task<ActionResult<RuleNameDto>> PostRuleName(RuleNameDto ruleName)
        {
            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var createdRuleName = await _repository.AddAsync(ruleName);
            return CreatedAtAction(nameof(GetRuleName), new { id = createdRuleName.Id }, createdRuleName);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRuleName(int id, RuleNameDto ruleName)
        {
            if (id != ruleName.Id)
                return BadRequest("The Id focused and the Id mentioned are different.");

            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var updated = await _repository.UpdateAsync(id, ruleName);
            if (!updated)
                return NotFound("The Id mentioned does not exist.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuleName(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound("The Id mentioned does not exist.");

            return NoContent();
        }
    }
}
