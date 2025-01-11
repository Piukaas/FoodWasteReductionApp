using System.ComponentModel.DataAnnotations;

namespace FoodWasteReduction.Web.Models.Auth
{
    public class RegisterCanteenStaffViewModel
    {
        [Required(ErrorMessage = "E-mail is verplicht")]
        [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Naam is verplicht")]
        [Display(Name = "Naam")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Personeelsnummer is verplicht")]
        [Display(Name = "Personeelsnummer")]
        public string PersonnelNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kantine is verplicht")]
        [Display(Name = "Kantine")]
        public int CanteenId { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        [MinLength(8, ErrorMessage = "Wachtwoord moet tenminste 8 karakters bevatten")]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bevestig je wachtwoord")]
        [Compare(nameof(Password), ErrorMessage = "Wachtwoorden komen niet overeen")]
        [DataType(DataType.Password)]
        [Display(Name = "Bevestig wachtwoord")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
