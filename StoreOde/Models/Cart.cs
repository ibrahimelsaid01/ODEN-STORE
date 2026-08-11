using System.ComponentModel.DataAnnotations;

namespace StoreOde.Models
{
    public partial class Cart
    {
        public const int UserIdMaxLength = 450;

        public int Id { get; set; }

        [Required(
            ErrorMessage = "A cart item must belong to a user.")]
        [StringLength(
            UserIdMaxLength,
            ErrorMessage = "User identifier cannot exceed 450 characters.")]
        public string UserId { get; set; } = string.Empty;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "The selected product is invalid.")]
        public int ProductId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Quantity must be greater than zero.")]
        public int Qty { get; set; }

        public virtual Product? Product { get; set; }
    }
}