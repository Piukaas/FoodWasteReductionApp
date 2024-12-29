using System.ComponentModel.DataAnnotations;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Validations;

namespace FoodWasteReduction.Web.Models.Auth
{
    public class RegisterStudentViewModel
    {
        [Required(ErrorMessage = "E-mail is verplicht")]
        [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Naam is verplicht")]
        [Display(Name = "Naam")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Studentnummer is verplicht")]
        [Display(Name = "Studentnummer")]
        public string StudentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Geboortedatum is verplicht")]
        [MinimumAge(16)]
        [Display(Name = "Geboortedatum")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Studiestad is verplicht")]
        [Display(Name = "Studiestad")]
        public City StudyCity { get; set; }

        [Required(ErrorMessage = "Telefoonnummer is verplicht")]
        [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
        [Display(Name = "Telefoonnummer")]
        public string PhoneNumber { get; set; } = string.Empty;

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
