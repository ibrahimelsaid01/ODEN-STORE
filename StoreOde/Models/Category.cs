using System.ComponentModel.DataAnnotations;

namespace StoreOde.Models
{
    public partial class Category
    {
        public const int NameMaxLength = 100;
        public const int IconClassMaxLength = 100;
        public const int DescriptionMaxLength = 500;

        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(
            NameMaxLength,
            ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(
            IconClassMaxLength,
            ErrorMessage = "Icon class cannot exceed 100 characters.")]
        public string? IconClass { get; set; }

        [StringLength(
            DescriptionMaxLength,
            ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
            = new HashSet<Product>();
    }
}