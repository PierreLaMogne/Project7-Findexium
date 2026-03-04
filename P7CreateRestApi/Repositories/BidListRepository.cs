using Microsoft.EntityFrameworkCore;
using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public class BidListRepository : IBidListRepository
    {
        private readonly LocalDbContext _context;
        public BidListRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BidListDto>> GetAllAsync()
        {
            return await _context.BidLists
                .Select(b => new BidListDto
                {
                    BidListId = b.BidListId,
                    Account = b.Account,
                    BidType = b.BidType,
                    BidQuantity = b.BidQuantity
                })
                .ToListAsync();
        }

        public async Task<BidListDto?> GetByIdAsync(int id)
        {
            var bidList = await _context.BidLists.FindAsync(id);
            if (bidList == null)
                return null;
            
            return new BidListDto
            {
                BidListId = bidList.BidListId,
                Account = bidList.Account,
                BidType = bidList.BidType,
                BidQuantity = bidList.BidQuantity
            };
        }

        public async Task<BidListDto> AddAsync(BidListDto dto)
        {
            var bidList = new BidList
            {
                Account = dto.Account,
                BidType = dto.BidType,
                BidQuantity = dto.BidQuantity
            };
            _context.BidLists.Add(bidList);
            await _context.SaveChangesAsync();

            dto.BidListId = bidList.BidListId;

            return dto;
        }

        public async Task<bool> UpdateAsync(int id, BidListDto dto)
        {
            var existingBidList = await _context.BidLists.FindAsync(id);
            if (existingBidList == null)
                return false;

            existingBidList.Account = dto.Account;
            existingBidList.BidType = dto.BidType;
            existingBidList.BidQuantity = dto.BidQuantity;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bidList = await _context.BidLists.FindAsync(id);
            if (bidList == null)
                return false;

            _context.BidLists.Remove(bidList);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
