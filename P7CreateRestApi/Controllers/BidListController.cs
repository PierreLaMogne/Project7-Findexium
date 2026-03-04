using Microsoft.AspNetCore.Mvc;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;

namespace FindexiumAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidListController : ControllerBase
    {
        private readonly IBidListRepository _repository;

        public BidListController(IBidListRepository bidListRepository)
        {
            _repository = bidListRepository;
        }

        // GET: api/BidList
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BidListDto>>> GetBidLists()
        {
            var bidLists = await _repository.GetAllAsync();
            return Ok(bidLists);
        }

        // GET: api/BidList/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BidListDto>> GetBidList(int id)
        {
            var bidList = await _repository.GetByIdAsync(id);
            if (bidList == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(bidList);
        }

        // PUT: api/BidList/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBidList(int id, BidListDto bidList)
        {
            if (id != bidList.BidListId)
                return BadRequest("The Id focused and the Id mentioned are different.");

            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var updated = await _repository.UpdateAsync(id, bidList);
            if (!updated)
                return NotFound("The Id mentioned does not exist.");

            return NoContent();
        }

        // POST: api/BidList
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BidListDto>> PostBidList(BidListDto bidList)
        {
            if (!ModelState.IsValid)
                return BadRequest("Informations mentionned are not valid.");

            var createdBidList = await _repository.AddAsync(bidList);
            return CreatedAtAction(nameof(GetBidList), new { id = createdBidList.BidListId }, createdBidList);
        }

        // DELETE: api/BidList/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBidList(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound("The Id mentioned does not exist.");

            return NoContent();
        }
    }
}
