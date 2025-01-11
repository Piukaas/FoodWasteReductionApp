using System.Text.Json.Nodes;
using FoodWasteReduction.Core.Constants;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Web.Attributes;
using FoodWasteReduction.Web.Models;
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
        public async Task<IActionResult> Manage(int? canteenId = null)
        {
            var canteens = await _canteenService.GetCanteens();

            var userData = HttpContext.Session.GetString("UserData");
            if (!string.IsNullOrEmpty(userData))
            {
                var jsonObject = JsonNode.Parse(userData);
                var staffCanteenId = jsonObject?["CanteenId"]?.GetValue<int>();
                if (staffCanteenId.HasValue)
                {
                    var staffCanteen = canteens.FirstOrDefault(c => c.Id == staffCanteenId.Value);
                    ViewData["StaffCanteen"] = staffCanteen?.Id;

                    if (!Request.Query.ContainsKey("canteenId"))
                    {
                        canteenId = staffCanteen?.Id;
                    }
                }
            }

            ViewData["Canteens"] = canteens;
            ViewData["SelectedCanteenId"] = canteenId;

            var packages = await _packageService.GetPackagesForManagement(canteenId);
            return View(packages);
        }

        [AuthorizeRole(Roles.CanteenStaff)]
        public async Task<IActionResult> Create()
        {
            ViewData["Products"] = await _packageService.GetProducts();
            ViewData["Canteens"] = await _canteenService.GetCanteens();

            return View(new PackageViewModel());
        }

        [HttpPost]
        [AuthorizeRole(Roles.CanteenStaff)]
        public async Task<IActionResult> Create(PackageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Products"] = await _packageService.GetProducts();
                ViewData["Canteens"] = await _canteenService.GetCanteens();
                return View(model);
            }

            try
            {
                await _packageService.CreatePackage(model);
                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewData["Products"] = await _packageService.GetProducts();
                ViewData["Canteens"] = await _canteenService.GetCanteens();
                return View(model);
            }
        }

        [AuthorizeRole(Roles.CanteenStaff)]
        public async Task<IActionResult> Edit(int id)
        {
            var package = await _packageService.GetPackage(id);

            if (package == null)
                return NotFound();

            ViewData["Products"] = await _packageService.GetProducts();
            ViewData["Canteens"] = await _canteenService.GetCanteens();

            var model = new PackageViewModel
            {
                Id = package.Id,
                Name = package.Name,
                Type = package.Type,
                City = package.City,
                PickupTime = package.PickupTime,
                ExpiryTime = package.ExpiryTime,
                Price = package.Price,
                CanteenId = package.Canteen?.Id ?? 0,
                ProductIds = package.Products?.Select(p => p.Id).ToList() ?? [],
            };

            return View(model);
        }

        [HttpPost]
        [AuthorizeRole(Roles.CanteenStaff)]
        public async Task<IActionResult> Edit(int id, PackageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Products"] = await _packageService.GetProducts();
                ViewData["Canteens"] = await _canteenService.GetCanteens();
                return View(model);
            }

            try
            {
                await _packageService.UpdatePackage(id, model);
                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewData["Products"] = await _packageService.GetProducts();
                ViewData["Canteens"] = await _canteenService.GetCanteens();
                return View(model);
            }
        }

        [HttpPost]
        [AuthorizeRole(Roles.CanteenStaff)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _packageService.DeletePackage(id);
                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Manage));
            }
        }

        [HttpPost]
        [AuthorizeRole(Roles.CanteenStaff)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductViewModel model)
        {
            try
            {
                var product = await _packageService.CreateProduct(model);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
