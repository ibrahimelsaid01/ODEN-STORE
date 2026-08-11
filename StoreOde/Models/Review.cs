using System.ComponentModel.DataAnnotations;

namespace StoreOde.Models
{
    public partial class Review
    {
        public const int NameMaxLength = 100;
        public const int EmailMaxLength = 100;
        public const int SubjectMaxLength = 200;
        public const int DescriptionMaxLength = 4000;

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(
            NameMaxLength,
            ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(
            EmailMaxLength,
            ErrorMessage = "Email address cannot exceed 100 characters.")]
        public string? Email { get; set; }

        [StringLength(
            SubjectMaxLength,
            ErrorMessage = "Subject cannot exceed 200 characters.")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(
            DescriptionMaxLength,
            ErrorMessage = "Description cannot exceed 4000 characters.")]
        public string Description { get; set; } = string.Empty;
    }
}