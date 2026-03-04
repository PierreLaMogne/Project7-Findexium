using Microsoft.AspNetCore.Mvc;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;

namespace FindexiumAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeController : ControllerBase
    {
        private readonly ITradeRepository _repository;
        public TradeController(ITradeRepository tradeRepository)
        {
            _repository = tradeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TradeDto>>> GetTrades()
        {
            var trades = await _repository.GetAllAsync();
            return Ok(trades);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TradeDto>> GetTrade(int id)
        {
            var trade = await _repository.GetByIdAsync(id);
            if (trade == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(trade);
        }

        [HttpPost]
        public async Task<ActionResult<TradeDto>> PostTrade(TradeDto trade)
        {
            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var createdTrade = await _repository.AddAsync(trade);
            return CreatedAtAction(nameof(GetTrade), new { id = createdTrade.TradeId }, createdTrade);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrade(int id, TradeDto trade)
        {
            if (id != trade.TradeId)
                return BadRequest("The Id focused and the Id mentioned are different.");

            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var updated = await _repository.UpdateAsync(id, trade);
            if (!updated)
                return NotFound("The Id mentioned does not exist.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrade(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound("The Id mentioned does not exist.");

            return NoContent();
        }
    }
}
