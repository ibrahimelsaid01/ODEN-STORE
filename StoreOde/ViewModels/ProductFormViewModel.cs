using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using StoreOde.Models;

namespace StoreOde.ViewModels
{
    public sealed class ProductFormViewModel : IValidatableObject
    {
        public const long ImageMaxFileSizeBytes =
            5 * 1024 * 1024;

        private static readonly HashSet<string>
            AllowedImageExtensions =
                new(
                    StringComparer.OrdinalIgnoreCase)
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };

        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(Product.NameMaxLength)]
        public string Name { get; set; } = string.Empty;

        [StringLength(Product.DescriptionMaxLength)]
        public string? Description { get; set; }

        [Range(
            typeof(decimal),
            "0.01",
            "79228162514264337593543950335",
            ErrorMessage =
                "Product price must be greater than zero.")]
        public decimal Price { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "The selected category is invalid.")]
        public int Catid { get; set; }

        [Display(Name = "Product Image")]
        public IFormFile? ImageFile { get; set; }

        [StringLength(Product.TypeMaxLength)]
        public string? Type { get; set; }

        [StringLength(Product.SupplierNameMaxLength)]
        public string? SupplierName { get; set; }

        [StringLength(Product.ReviewUrlMaxLength)]
        public string? ReviewUrl { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessage =
                "Product quantity cannot be negative.")]
        public int Quantity { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "79228162514264337593543950335",
            ErrorMessage =
                "Discounted price cannot be negative.")]
        public decimal? Priceafterdiscount { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (Priceafterdiscount is not null &&
                Priceafterdiscount > Price)
            {
                yield return new ValidationResult(
                    "Discounted price cannot be greater than the original price.",
                    new[]
                    {
                        nameof(Priceafterdiscount)
                    });
            }

            if (ImageFile is null)
            {
                yield break;
            }

            if (ImageFile.Length <= 0)
            {
                yield return new ValidationResult(
                    "The selected product image is empty.",
                    new[]
                    {
                        nameof(ImageFile)
                    });

                yield break;
            }

            if (ImageFile.Length > ImageMaxFileSizeBytes)
            {
                yield return new ValidationResult(
                    "The product image cannot exceed 5 MB.",
                    new[]
                    {
                        nameof(ImageFile)
                    });
            }

            var extension =
                Path.GetExtension(
                    ImageFile.FileName);

            if (string.IsNullOrWhiteSpace(extension) ||
                !AllowedImageExtensions.Contains(extension))
            {
                yield return new ValidationResult(
                    "Only JPG, JPEG, PNG, and WEBP images are allowed.",
                    new[]
                    {
                        nameof(ImageFile)
                    });
            }
        }
    }
}