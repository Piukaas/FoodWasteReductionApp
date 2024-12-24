using System.ComponentModel.DataAnnotations;

namespace FoodWasteReduction.Core.Validation
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value is DateTime dateOfBirth)
            {
                var today = DateTime.Today;
                var age = today.Year - dateOfBirth.Year;

                if (dateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }

                if (dateOfBirth > today || age < _minimumAge)
                {
                    return new ValidationResult(
                        $"Je moet tenminste {_minimumAge} jaar oud zijn en de datum mag niet in de toekomst liggen"
                    );
                }
            }
            return ValidationResult.Success;
        }
    }
}
