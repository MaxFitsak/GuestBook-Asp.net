using WebGuestBook.Models.Review;

namespace WebGuestBook.Repositories
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllAsync();
        Task AddAsync(Review review);
        Task SaveChangesAsync();
    }
}