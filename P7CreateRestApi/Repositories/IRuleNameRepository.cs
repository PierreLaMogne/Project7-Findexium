using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public interface IRuleNameRepository
    {
        Task<IEnumerable<RuleNameDto>> GetAllAsync();
        Task<RuleNameDto?> GetByIdAsync(int id);
        Task<RuleNameDto> AddAsync(RuleNameDto dto);
        Task<bool> UpdateAsync(int id, RuleNameDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
