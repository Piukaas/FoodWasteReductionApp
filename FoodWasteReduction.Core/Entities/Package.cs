using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodWasteReduction.Core.Enums;
using FoodWasteReduction.Core.Validations;

namespace FoodWasteReduction.Core.Entities
{
    public class Package
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public City City { get; set; }

        [Required]
        public int CanteenId { get; set; }

        [ForeignKey("CanteenId")]
        public virtual Canteen? Canteen { get; set; }

        [Required]
        public MealType Type { get; set; }

        [Required]
        [FutureDate]
        public DateTime PickupTime { get; set; }

        [Required]
        public DateTime ExpiryTime { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public bool Is18Plus { get; set; } = false;

        public string? ReservedById { get; set; }

        [ForeignKey("ReservedById")]
        public virtual Student? ReservedBy { get; set; }

        public virtual ICollection<Product> Products { get; set; } = [];
    }
}
