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

        [AuthorizeRole(Roles.Student, Roles.CanteenStaff)]
        public async Task<IActionResult> Index(City? city = null, MealType? type = null)
        {
            var packages = await _packageService.GetAvailablePackages(city, type);
            return View(packages);
        }
    }
}
