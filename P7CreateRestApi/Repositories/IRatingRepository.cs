using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public interface IRatingRepository
    {
        Task<IEnumerable<RatingDto>> GetAllAsync();
        Task<RatingDto?> GetByIdAsync(int id);
        Task<RatingDto> AddAsync(RatingDto dto);
        Task<bool> UpdateAsync(int id, RatingDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
