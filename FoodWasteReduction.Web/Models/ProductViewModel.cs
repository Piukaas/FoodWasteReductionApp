using System.ComponentModel.DataAnnotations;

namespace FoodWasteReduction.Web.Models
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Naam is verplicht")]
        [Display(Name = "Naam")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Bevat alcohol")]
        public bool ContainsAlcohol { get; set; } = false;

        [Display(Name = "Afbeelding URL")]
        [Url(ErrorMessage = "Voer een geldige URL in")]
        public string? ImageUrl { get; set; }
    }
}
