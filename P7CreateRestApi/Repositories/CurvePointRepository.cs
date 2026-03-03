using Microsoft.EntityFrameworkCore;
using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public class CurvePointRepository : ICurvePointRepository
    {
        private readonly LocalDbContext _context;
        public CurvePointRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CurvePointDto>> GetAllAsync()
        {
            return await _context.CurvePoints
                .Select(c => new CurvePointDto
                {
                    Id = c.Id,
                    Term = c.Term,
                    CurvePointValue = c.CurvePointValue
                })
                .ToListAsync();
        }

        public async Task<CurvePointDto?> GetByIdAsync(int id)
        {
            var curvePoint = await _context.CurvePoints.FindAsync(id);
            if (curvePoint == null)
                return null;
            
            return new CurvePointDto
            {
                Id = curvePoint.Id,
                CurveId = curvePoint.CurveId,
                Term = curvePoint.Term,
                CurvePointValue = curvePoint.CurvePointValue
            };
        }

        public async Task<CurvePointDto> AddAsync(CurvePointDto dto)
        {
            var curvePoint = new CurvePoint
            {
                CurveId = dto.CurveId,
                Term = dto.Term,
                CurvePointValue = dto.CurvePointValue
            };

            _context.CurvePoints.Add(curvePoint);
            await _context.SaveChangesAsync();

            dto.Id = curvePoint.Id;

            return dto;
        }

        public async Task<bool> UpdateAsync(int id, CurvePointDto dto)
        {
            var curvePoint = await _context.CurvePoints.FindAsync(id);
            if (curvePoint == null)
                return false;

            curvePoint.CurvePointValue = dto.CurvePointValue;
            curvePoint.Term = dto.Term;
            curvePoint.CurvePointValue = dto.CurvePointValue;

            _context.CurvePoints.Update(curvePoint);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var curvePoint = await _context.CurvePoints.FindAsync(id);
            if (curvePoint == null)
                return false;

            _context.CurvePoints.Remove(curvePoint);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
