using System.Text.Json.Nodes;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Web.Controllers
{
    public class PackagesController(IPackageService packageService) : Controller
    {
        private readonly IPackageService _packageService = packageService;

        public async Task<IActionResult> Index(City? city = null, MealType? type = null)
        {
            DateTime? dateOfBirth = null;
            var userData = HttpContext.Session.GetString("UserData");

            if (!string.IsNullOrEmpty(userData))
            {
                Console.WriteLine($"UserData from session: {userData}");
                var jsonObject = JsonNode.Parse(userData);

                var studyCityValue = jsonObject?["StudyCity"]?.GetValue<int>();
                if (studyCityValue.HasValue && !Request.Query.ContainsKey("city"))
                {
                    city = (City)studyCityValue.Value;
                }

                var dateString = jsonObject?["DateOfBirth"]?.GetValue<string>();
                if (!string.IsNullOrEmpty(dateString))
                {
                    dateOfBirth = DateTime.Parse(dateString);
                }
            }

            ViewData["UserCity"] = city;
            ViewData["UserDateOfBirth"] = dateOfBirth;
            ViewData["SelectedType"] = type;

            var packages = await _packageService.GetAvailablePackages(city, type);
            return View(packages);
        }
    }
}
