using FoodWasteReduction.Web.Models.Auth;
using FoodWasteReduction.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FoodWasteReduction.Web.Controllers
{
    public class AuthController(IAuthService authService, IAuthGuardService authGuardService)
        : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly IAuthGuardService _authGuardService = authGuardService;

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

            var (success, token, userData) = await _authService.Login(model);
            if (success)
            {
                _authGuardService.SetToken(token);
                HttpContext.Session.SetString("JWTToken", token);
                HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(userData));

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Verkeerde login gegevens");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            _authGuardService.ClearToken();
            HttpContext.Session.Remove("JWTToken");
            HttpContext.Session.Remove("UserData");
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

            ModelState.AddModelError(string.Empty, "Registratie gefaald");
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

            ModelState.AddModelError(string.Empty, "Registratie gefaald");
            return View(model);
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
