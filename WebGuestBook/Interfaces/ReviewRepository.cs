using Microsoft.EntityFrameworkCore;
using WebGuestBook.Data;
using WebGuestBook.Models.Review;

namespace WebGuestBook.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly UserContext _context;

        public ReviewRepository(UserContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}