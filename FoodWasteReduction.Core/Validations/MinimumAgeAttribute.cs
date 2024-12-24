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
                        $"Person has to be atleast {_minimumAge} years old and the date should not be in the future."
                    );
                }
            }
            return ValidationResult.Success;
        }
    }
}
