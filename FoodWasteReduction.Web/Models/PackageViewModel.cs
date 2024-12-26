using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Web.Models
{
    public class PackageViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Naam is verplicht")]
        [Display(Name = "Naam")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stad is verplicht")]
        [Display(Name = "Stad")]
        public City City { get; set; }

        [Required(ErrorMessage = "Kantine is verplicht")]
        [Display(Name = "Kantine")]
        public int CanteenId { get; set; }

        [Required(ErrorMessage = "Type is verplicht")]
        [Display(Name = "Type maaltijd")]
        public MealType Type { get; set; }

        [Required(ErrorMessage = "Ophaaltijd is verplicht")]
        [Display(Name = "Ophalen vanaf")]
        public DateTime PickupTime { get; set; }

        [Required(ErrorMessage = "Vervaltijd is verplicht")]
        [Display(Name = "Ophalen tot")]
        public DateTime ExpiryTime { get; set; }

        [Required(ErrorMessage = "Prijs is verplicht")]
        [Range(0, double.MaxValue, ErrorMessage = "Prijs moet positief zijn")]
        [Display(Name = "Prijs")]
        public decimal Price { get; set; }

        [Display(Name = "18+ pakket")]
        public bool Is18Plus { get; set; } = false;

        [Display(Name = "Producten")]
        public List<int> ProductIds { get; set; } = [];
    }
}
