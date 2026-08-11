using System.ComponentModel.DataAnnotations;
using StoreOde.Models;

namespace StoreOde.ViewModels
{
    public sealed class ReviewFormViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(
            Review.NameMaxLength,
            ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(
            Review.SubjectMaxLength,
            ErrorMessage = "Subject cannot exceed 200 characters.")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Review is required.")]
        [StringLength(
            Review.DescriptionMaxLength,
            MinimumLength = 10,
            ErrorMessage =
                "Review must be between 10 and 4000 characters.")]
        public string Description { get; set; } = string.Empty;
    }
}