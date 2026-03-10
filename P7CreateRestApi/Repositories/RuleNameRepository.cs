using Microsoft.EntityFrameworkCore;
using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public class RuleNameRepository : IRuleNameRepository
    {
        private readonly LocalDbContext _context;

        public RuleNameRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RuleNameDto>> GetAllAsync()
        {
            return await _context.RuleNames
                .Select(r => new RuleNameDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Json = r.Json,
                    Template = r.Template,
                    SqlStr = r.SqlStr,
                    SqlPart = r.SqlPart
                })
                .ToListAsync();
        }

        public async Task<RuleNameDto?> GetByIdAsync(int id)
        {
            var ruleName = await _context.RuleNames.FindAsync(id);
            if (ruleName == null)
                return null;

            return new RuleNameDto
            {
                Id = ruleName.Id,
                Name = ruleName.Name,
                Description = ruleName.Description,
                Json = ruleName.Json,
                Template = ruleName.Template,
                SqlStr = ruleName.SqlStr,
                SqlPart = ruleName.SqlPart
            };
        }

        public async Task<RuleNameDto> AddAsync(RuleNameDto dto)
        {
            var ruleName = new RuleName
            {
                Name = dto.Name,
                Description = dto.Description,
                Json = dto.Json,
                Template = dto.Template,
                SqlStr = dto.SqlStr,
                SqlPart = dto.SqlPart
            };

            _context.RuleNames.Add(ruleName);
            await _context.SaveChangesAsync();

            dto.Id = ruleName.Id;

            return dto;
        }

        public async Task<bool> UpdateAsync(int id, RuleNameDto dto)
        {
            var existingRuleName = await _context.RuleNames.FindAsync(id);
            if (existingRuleName == null)
                return false;

            existingRuleName.Name = dto.Name;
            existingRuleName.Description = dto.Description;
            existingRuleName.Json = dto.Json;
            existingRuleName.Template = dto.Template;
            existingRuleName.SqlStr = dto.SqlStr;
            existingRuleName.SqlPart = dto.SqlPart;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ruleName = await _context.RuleNames.FindAsync(id);
            if (ruleName == null)
                return false;

            _context.RuleNames.Remove(ruleName);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
