using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public interface ICurvePointRepository
    {
        Task<IEnumerable<CurvePointDto>> GetAllAsync();
        Task<CurvePointDto?> GetByIdAsync(int id);
        Task<CurvePointDto> AddAsync(CurvePointDto dto);
        Task<bool> UpdateAsync(int id, CurvePointDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
