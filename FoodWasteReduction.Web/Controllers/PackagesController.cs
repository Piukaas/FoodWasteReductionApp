using System.Text.Json.Nodes;
using FoodWasteReduction.Core.Constants;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Attributes;
using FoodWasteReduction.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Web.Controllers
{
    public class PackagesController(IPackageService packageService, ICanteenService canteenService)
        : Controller
    {
        private readonly IPackageService _packageService = packageService;
        private readonly ICanteenService _canteenService = canteenService;

        [AuthorizeRole(Roles.Student)]
        public async Task<IActionResult> Index(City? city = null, MealType? type = null)
        {
            DateTime? dateOfBirth = null;
            var userData = HttpContext.Session.GetString("UserData");

            if (!string.IsNullOrEmpty(userData))
            {
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

        [AuthorizeRole(Roles.Student)]
        public async Task<IActionResult> Reservations()
        {
            var userData = HttpContext.Session.GetString("UserData");
            string? userId = null;

            if (!string.IsNullOrEmpty(userData))
            {
                var jsonObject = JsonNode.Parse(userData);
                userId = jsonObject?["Id"]?.GetValue<string>();
            }

            var packages = await _packageService.GetReservedPackages(userId);
            return View(packages);
        }

        [AuthorizeRole(Roles.CanteenStaff)]
        public async Task<IActionResult> ManagePackages(int? canteenId = null)
        {
            var canteens = await _canteenService.GetCanteens();

            if (!Request.Query.ContainsKey("canteenId"))
            {
                var userData = HttpContext.Session.GetString("UserData");
                if (!string.IsNullOrEmpty(userData))
                {
                    var jsonObject = JsonNode.Parse(userData);
                    var location = jsonObject?["Location"]?.GetValue<int>();
                    if (location.HasValue)
                    {
                        var staffCanteen = canteens.FirstOrDefault(c =>
                            (int)c.Location == location.Value
                        );
                        canteenId = staffCanteen?.Id;
                    }
                }
            }

            ViewData["Canteens"] = canteens;
            ViewData["SelectedCanteenId"] = canteenId;

            var packages = await _packageService.GetPackagesForManagement(canteenId);
            return View(packages);
        }
    }
}
