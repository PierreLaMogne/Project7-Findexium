using Microsoft.AspNetCore.Mvc;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Policy = "Users")]
        public async Task<ActionResult<IEnumerable<BidListDto>>> GetBidLists()
        {
            var bidLists = await _repository.GetAllAsync();
            if(!bidLists.Any())
                return NotFound("No BidList found.");

            return Ok(bidLists);
        }

        // GET: api/BidList/5
        [HttpGet("{id}")]
        [Authorize(Policy = "Users")]
        public async Task<ActionResult<BidListDto>> GetBidList(int id)
        {
            var bidList = await _repository.GetByIdAsync(id);
            if (bidList == null)
                return NotFound("The Id mentioned does not exist.");

            return Ok(bidList);
        }

        // POST: api/BidList
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BidListDto>> PostBidList(BidListDto bidList)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBidList = await _repository.AddAsync(bidList);
            return CreatedAtAction(nameof(GetBidList), new { id = createdBidList.BidListId }, createdBidList);
        }

        // PUT: api/BidList/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutBidList(int id, BidListDto bidList)
        {
            if (id != bidList.BidListId)
                return BadRequest("The Id focused and the Id mentioned are different.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _repository.UpdateAsync(id, bidList);
            if (!updated)
                return NotFound("The Id mentioned does not exist.");

            return Ok("The BidList mentioned has been updated.");
        }

        // DELETE: api/BidList/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBidList(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound("The Id mentioned does not exist.");

            return Ok("The BidList mentioned has been deleted.");
        }
    }
}
