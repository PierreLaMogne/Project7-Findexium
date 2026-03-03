using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public interface ITradeRepository
    {
        Task<IEnumerable<TradeDto>> GetAllAsync();
        Task<TradeDto?> GetByIdAsync(int id);
        Task<TradeDto> AddAsync(TradeDto dto);
        Task<bool> UpdateAsync(int id, TradeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
