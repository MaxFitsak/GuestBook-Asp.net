using Microsoft.EntityFrameworkCore;
using WebGuestBook.Models;
using WebGuestBook.Models.Review;
using WebGuestBook.Models.User;

namespace WebGuestBook.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<WebGuestBook.Models.Review.Review> Reviews => Set<WebGuestBook.Models.Review.Review>();
}