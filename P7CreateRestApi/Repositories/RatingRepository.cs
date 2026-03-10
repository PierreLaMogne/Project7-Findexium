using Microsoft.EntityFrameworkCore;
using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly LocalDbContext _context;

        public RatingRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RatingDto>> GetAllAsync()
        {
            return await _context.Ratings
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    MoodysRating = r.MoodysRating,
                    SandPRating = r.SandPRating,
                    FitchRating = r.FitchRating,
                    OrderNumber = r.OrderNumber
                })
                .ToListAsync();
        }

        public async Task<RatingDto?> GetByIdAsync(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
                return null;

            return new RatingDto
            {
                Id = rating.Id,
                MoodysRating = rating.MoodysRating,
                SandPRating = rating.SandPRating,
                FitchRating = rating.FitchRating,
                OrderNumber = rating.OrderNumber
            };
        }

        public async Task<RatingDto> AddAsync(RatingDto dto)
        {
            var rating = new Rating
            {
                MoodysRating = dto.MoodysRating,
                SandPRating = dto.SandPRating,
                FitchRating = dto.FitchRating,
                OrderNumber = dto.OrderNumber
            };
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            dto.Id = rating.Id;

            return dto;
        }

        public async Task<bool> UpdateAsync(int id, RatingDto dto)
        {
            var existingRating = await _context.Ratings.FindAsync(id);
            if (existingRating == null)
                return false;

            existingRating.MoodysRating = dto.MoodysRating;
            existingRating.SandPRating = dto.SandPRating;
            existingRating.FitchRating = dto.FitchRating;
            existingRating.OrderNumber = dto.OrderNumber;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
                return false;

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
