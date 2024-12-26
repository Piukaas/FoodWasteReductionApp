using System.Text.Json.Nodes;
using FoodWasteReduction.Core.Constants;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Attributes;
using FoodWasteReduction.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteReduction.Web.Controllers
{
    public class PackagesController(IPackageService packageService) : Controller
    {
        private readonly IPackageService _packageService = packageService;

        [AuthorizeRole(Roles.Student)]
        public async Task<IActionResult> Index(City? city = null, MealType? type = null)
        {
            if (!Request.Query.ContainsKey("city"))
            {
                var userData = HttpContext.Session.GetString("UserData");
                if (!string.IsNullOrEmpty(userData))
                {
                    var jsonObject = JsonNode.Parse(userData);
                    var studyCityValue = jsonObject?["StudyCity"]?.GetValue<int>();
                    if (studyCityValue.HasValue)
                    {
                        city = (City)studyCityValue.Value;
                    }
                }
            }

            ViewData["UserCity"] = city;
            ViewData["SelectedType"] = type;
            var packages = await _packageService.GetAvailablePackages(city, type);
            return View(packages);
        }
    }
}
