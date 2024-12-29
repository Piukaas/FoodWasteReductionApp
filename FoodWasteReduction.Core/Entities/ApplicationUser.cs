using Microsoft.AspNetCore.Identity;

namespace FoodWasteReduction.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public override string? PhoneNumber { get; set; } = string.Empty;
    }
}
