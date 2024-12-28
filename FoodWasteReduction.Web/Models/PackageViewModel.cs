using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Validations;

namespace FoodWasteReduction.Web.Models
{
    public class PackageViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Naam is verplicht")]
        [Display(Name = "Naam")]
        public string Name { get; set; } = string.Empty;

        public City? City { get; set; }

        public int? CanteenId { get; set; }

        [Required(ErrorMessage = "Type is verplicht")]
        [Display(Name = "Type maaltijd")]
        public MealType Type { get; set; }

        [Required(ErrorMessage = "Ophaaltijd is verplicht")]
        [Display(Name = "Ophalen vanaf")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "Ophaaltijd mag maximaal 2 dagen vooruit liggen")]
        public DateTime PickupTime { get; set; } =
            DateTime.Now.Date.AddHours(DateTime.Now.Hour + 1);

        [Display(Name = "Ophalen tot")]
        public DateTime ExpiryTime { get; set; } =
            DateTime.Now.Date.AddHours(DateTime.Now.Hour + 3);

        [Required(ErrorMessage = "Prijs is verplicht")]
        [Range(0, double.MaxValue, ErrorMessage = "Prijs moet positief zijn")]
        [Display(Name = "Prijs")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Kies tenminste 1 product")]
        [Display(Name = "Producten")]
        public List<int> ProductIds { get; set; } = [];
    }
}
