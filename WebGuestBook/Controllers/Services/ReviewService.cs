using Microsoft.EntityFrameworkCore;
using WebGuestBook.Data;
using WebGuestBook.Models;
using WebGuestBook.Models.Review;
using WebGuestBook.Models.User;

namespace WebGuestBook.Services;

    public class ReviewService
    {
        private readonly UserContext _db;

        public ReviewService(UserContext db)
        {
            _db = db;
        }
        
        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _db.Reviews
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }
        
        public async Task AddReviewAsync(int userId, string authorName, string message)
        {
            var review = new Review
            {
                UserId = userId,
                Author = authorName,
                Message = message.Trim(),
                Date = DateTime.UtcNow
            };

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();
        }
    }
