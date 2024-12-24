using FoodWasteReduction.Web.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAuthGuardService _authGuardService;

        public AuthController(IAuthService authService, IAuthGuardService authGuardService)
        {
            _authService = authService;
            _authGuardService = authGuardService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_authGuardService.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, token) = await _authService.Login(model);
            if (success)
            {
                _authGuardService.SetToken(token);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            _authGuardService.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult RegisterStudent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStudent(RegisterStudentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.RegisterStudent(model);
            if (result)
                return RedirectToAction(nameof(Login));

            ModelState.AddModelError(string.Empty, "Registration failed");
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterCanteenStaff()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCanteenStaff(RegisterCanteenStaffViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.RegisterCanteenStaff(model);
            if (result)
                return RedirectToAction(nameof(Login));

            ModelState.AddModelError(string.Empty, "Registration failed");
            return View(model);
        }
    }
}