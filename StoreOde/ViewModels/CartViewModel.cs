namespace StoreOde.ViewModels
{
    public sealed class CartViewModel
    {
        public IReadOnlyList<CartItemViewModel> Items { get; init; }
            = Array.Empty<CartItemViewModel>();

        public bool IsEmpty =>
            Items.Count == 0;

        public int TotalQuantity =>
            Items.Sum(
                item => item.Quantity);

        public decimal Subtotal =>
            Items.Sum(
                item => item.LineTotal);
    }

    public sealed class CartItemViewModel
    {
        public int CartItemId { get; init; }

        public int ProductId { get; init; }

        public string ProductName { get; init; }
            = string.Empty;

        public string? Photo { get; init; }

        public decimal UnitPrice { get; init; }

        public decimal? OriginalPrice { get; init; }

        public int Quantity { get; init; }

        public int AvailableStock { get; init; }

        public decimal LineTotal =>
            UnitPrice * Quantity;

        public bool HasDiscount =>
            OriginalPrice.HasValue
            &&
            OriginalPrice.Value > UnitPrice;

        public bool IsInStock =>
            AvailableStock > 0;

        public bool CanIncreaseQuantity =>
            IsInStock
            &&
            Quantity < AvailableStock;
    }
}