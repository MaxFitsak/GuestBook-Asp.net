using WebGuestBook.Models.Review;
using WebGuestBook.Repositories;

namespace WebGuestBook.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _reviewRepository.GetAllAsync();
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

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();
        }
    }
}