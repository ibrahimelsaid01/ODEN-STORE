using System.ComponentModel.DataAnnotations;

namespace StoreOde.Models
{
    public partial class Product : IValidatableObject
    {
        public const int NameMaxLength = 200;
        public const int DescriptionMaxLength = 2000;
        public const int PhotoMaxLength = 500;
        public const int TypeMaxLength = 50;
        public const int SupplierNameMaxLength = 100;
        public const int ReviewUrlMaxLength = 500;

        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLength)]
        public string Name { get; set; } = string.Empty;

        [StringLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        public decimal Price { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "The selected category is invalid.")]
        public int Catid { get; set; }

        [StringLength(PhotoMaxLength)]
        public string? Photo { get; set; }

        [StringLength(TypeMaxLength)]
        public string? Type { get; set; }

        [StringLength(SupplierNameMaxLength)]
        public string? SupplierName { get; set; }

        public DateTime? EntryDate { get; set; }

        [StringLength(ReviewUrlMaxLength)]
        public string? ReviewUrl { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "Product quantity cannot be negative.")]
        public int Quantity { get; set; }

        public decimal? Priceafterdiscount { get; set; }

        public virtual Category? Cat { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
            = new HashSet<Cart>();

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult(
                    "Product name is required.",
                    new[] { nameof(Name) });
            }

            if (Price <= 0)
            {
                yield return new ValidationResult(
                    "Product price must be greater than zero.",
                    new[] { nameof(Price) });
            }

            if (Catid <= 0)
            {
                yield return new ValidationResult(
                    "The selected category is invalid.",
                    new[] { nameof(Catid) });
            }

            if (Priceafterdiscount is < 0)
            {
                yield return new ValidationResult(
                    "Discounted price cannot be negative.",
                    new[] { nameof(Priceafterdiscount) });
            }

            if (Priceafterdiscount is not null &&
                Priceafterdiscount > Price)
            {
                yield return new ValidationResult(
                    "Discounted price cannot be greater than the original price.",
                    new[] { nameof(Priceafterdiscount) });
            }
        }
    }
}