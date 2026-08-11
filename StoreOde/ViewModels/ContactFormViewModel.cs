using System.ComponentModel.DataAnnotations;

namespace StoreOde.ViewModels
{
    public sealed class ContactFormViewModel
    {
        public const int NameMaxLength = 100;
        public const int EmailMaxLength = 256;
        public const int SubjectMaxLength = 200;
        public const int MessageMaxLength = 4000;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(
            NameMaxLength,
            ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(
            EmailMaxLength,
            ErrorMessage = "Email cannot exceed 256 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(
            SubjectMaxLength,
            ErrorMessage = "Subject cannot exceed 200 characters.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(
            MessageMaxLength,
            ErrorMessage = "Message cannot exceed 4000 characters.")]
        public string Message { get; set; } = string.Empty;
    }
}