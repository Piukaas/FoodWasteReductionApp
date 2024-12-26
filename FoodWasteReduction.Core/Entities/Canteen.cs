using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodWasteReduction.Core.Enums;

namespace FoodWasteReduction.Core.Entities
{
    public class Canteen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public City City { get; set; }

        [Required]
        public Location Location { get; set; }

        public bool ServesWarmMeals { get; set; } = false;
    }
}
