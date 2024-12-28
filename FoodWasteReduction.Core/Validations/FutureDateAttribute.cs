using System.ComponentModel.DataAnnotations;

namespace FoodWasteReduction.Core.Validations
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                var now = DateTime.Now;
                var maxDate = now.AddDays(2);
                return date > now && date <= maxDate;
            }
            return false;
        }
    }
}
