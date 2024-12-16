namespace FoodWasteReduction.Core.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string StudyCity { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}