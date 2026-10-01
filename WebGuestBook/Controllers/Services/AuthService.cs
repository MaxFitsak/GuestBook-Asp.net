using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using WebGuestBook.Models.User;
using WebGuestBook.Repositories;

namespace WebGuestBook.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Register(string email, string password, string firstName, string lastName)
        {
            if (await _userRepository.ExistsByEmailAsync(email))
            {
                return false;
            }

            var user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = string.Empty
            };

            user.Password = _hasher.HashPassword(user, password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            await SignInUser(user);
            return true;
        }

        public async Task<bool> Login(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || string.IsNullOrEmpty(user.Password))
            {
                return false;
            }

            var verifyResult = _hasher.VerifyHashedPassword(user, user.Password, password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return false;
            }

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