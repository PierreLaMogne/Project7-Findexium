using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Policy = "Users")]
        public async Task<ActionResult<IEnumerable<TradeDto>>> GetTrades()
        {
            var trades = await _repository.GetAllAsync();
            if (!trades.Any())
                return NotFound("No Trade found.");

            return Ok(trades);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Users")]
        public async Task<ActionResult<TradeDto>> GetTrade(int id)
        {
            var trade = await _repository.GetByIdAsync(id);
            if (trade == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(trade);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TradeDto>> PostTrade(TradeDto trade)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdTrade = await _repository.AddAsync(trade);
            return CreatedAtAction(nameof(GetTrade), new { id = createdTrade.TradeId }, createdTrade);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTrade(int id, TradeDto trade)
        {
            if (id != trade.TradeId)
                return BadRequest("The Id focused and the Id mentioned are different.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedTrade = await _repository.UpdateAsync(id, trade);
            if (!updatedTrade)
                return NotFound("The Id mentioned does not exist.");

            return Ok("The Trade mentioned has been updated.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTrade(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound("The Id mentioned does not exist.");

            return Ok("The Trade mentioned has been deleted.");
        }
    }
}
