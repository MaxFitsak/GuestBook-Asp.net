using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGuestBook.Models.Review;
using WebGuestBook.Services;

namespace WebGuestBook.Controllers
{
    public class GuestBookController : Controller
    {
        private readonly AuthService _authService;
        private readonly ReviewService _reviewService;

        public GuestBookController(AuthService authService, ReviewService reviewService)
        {
            _authService = authService;
            _reviewService = reviewService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return View(reviews);
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(Review review)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim.Value, out int userId))
            {
                return Challenge();
            }
            
            review.UserId = userId;
            review.Author = User.Identity?.Name ?? "Користувач";
            review.Date = DateTime.UtcNow;
            
            ModelState.Remove(nameof(review.Author));
            ModelState.Remove(nameof(review.User));

            if (!ModelState.IsValid)
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                return View("Index", reviews);
            }

            await _reviewService.AddReviewAsync(review.UserId, review.Author, review.Message);

            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public IActionResult Register() => User.Identity?.IsAuthenticated == true ? RedirectToAction(nameof(Index)) : View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string confirmPassword, string firstName, string lastName)
        {

            if (await _authService.Register(email, password, firstName, lastName))
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Користувач з такою поштою вже існує.");
            return View();
        }

        [HttpGet]
        public IActionResult Login() => User.Identity?.IsAuthenticated == true ? RedirectToAction(nameof(Index)) : View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (await _authService.Login(email, password))
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Невірний логін або пароль.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();
            return RedirectToAction(nameof(Index));
        }
    }
}