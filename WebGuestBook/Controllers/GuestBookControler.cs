using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> AddReview(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return RedirectToAction(nameof(Index));
            }

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim.Value, out int userId))
            {
                return Challenge();
            }

            string authorName = User.Identity?.Name ?? "Користувач";
            
            await _reviewService.AddReviewAsync(userId, authorName, message);

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