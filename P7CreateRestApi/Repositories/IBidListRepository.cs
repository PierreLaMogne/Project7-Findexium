using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public interface IBidListRepository
    {
        Task<IEnumerable<BidListDto>> GetAllAsync();
        Task<BidListDto?> GetByIdAsync(int id);
        Task<BidListDto> AddAsync(BidListDto dto);
        Task<bool> UpdateAsync(int id, BidListDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
