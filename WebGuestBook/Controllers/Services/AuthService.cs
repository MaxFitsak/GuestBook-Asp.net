using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore;
using WebGuestBook.Data;
using WebGuestBook.Models;
using WebGuestBook.Models.User;

namespace WebGuestBook.Services
{
    public class AuthService
    {
        private readonly UserContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthService(UserContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<bool> Register(string email, string password, string firstName, string lastName)
        {
            bool exists = await _db.Users.AnyAsync(u => u.Email == email);
            if (exists) return false;

            var user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = string.Empty
            };
            
            user.Password = _hasher.HashPassword(user, password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            
            await SignInUser(user);
            return true;
        }
        
        public async Task<bool> Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;
            
            var verifyResult = _hasher.VerifyHashedPassword(user, user.Password, password);
            if (verifyResult == PasswordVerificationResult.Failed) return false;

            await SignInUser(user);
            return true;
        }
        
        public async Task Logout()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
        
        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
        }
    }
}