using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StoreOde.Models;
using StoreOde.ViewModels;

namespace StoreOde.Controllers
{
    [Authorize]
    public sealed class CartController : Controller
    {
        private const string SuccessMessageKey =
            "CartSuccessMessage";

        private const string ErrorMessageKey =
            "CartErrorMessage";

        private readonly SouqcomContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(
            SouqcomContext context,
            ILogger<CartController> logger)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(logger);

            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId is null)
            {
                return Challenge();
            }

            var cartRows =
                await _context.Carts
                    .AsNoTracking()
                    .Where(cart =>
                        cart.UserId == userId)
                    .Include(cart =>
                        cart.Product)
                    .OrderBy(cart =>
                        cart.Id)
                    .ToListAsync(
                        cancellationToken);

            var items =
                new List<CartItemViewModel>(
                    cartRows.Count);

            foreach (var cartRow in cartRows)
            {
                if (cartRow.ProductId <= 0 ||
                    cartRow.Product is null)
                {
                    _logger.LogWarning(
                        "A cart row was skipped because " +
                        "its product reference was unavailable.");

                    continue;
                }

                var quantity =
                    cartRow.Qty;

                if (quantity <= 0)
                {
                    _logger.LogWarning(
                        "A cart row was skipped because " +
                        "its quantity was invalid.");

                    continue;
                }

                if (!TryResolvePricing(
                        cartRow.Product,
                        out var unitPrice,
                        out var originalPrice))
                {
                    _logger.LogWarning(
                        "A cart row was skipped because " +
                        "its product pricing was invalid.");

                    continue;
                }

                var availableStock =
                    ResolveAvailableStock(
                        cartRow.Product.Quantity);

                items.Add(
                    new CartItemViewModel
                    {
                        CartItemId =
                            cartRow.Id,

                        ProductId =
                            cartRow.Product.Id,

                        ProductName =
                            cartRow.Product.Name?.Trim()
                            ?? "Product",

                        Photo =
                            NormalizeOptionalText(
                                cartRow.Product.Photo),

                        UnitPrice =
                            unitPrice,

                        OriginalPrice =
                            originalPrice,

                        Quantity =
                            quantity,

                        AvailableStock =
                            availableStock
                    });
            }

            var model =
                new CartViewModel
                {
                    Items = items
                };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(
            int productId,
            int quantity = 1,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            if (userId is null)
            {
                return Challenge();
            }

            if (productId <= 0)
            {
                SetErrorMessage(
                    "The selected product is invalid.");

                return RedirectToAction(
                    nameof(Index));
            }

            if (quantity <= 0)
            {
                SetErrorMessage(
                    "Quantity must be greater than zero.");

                return RedirectToAction(
                    nameof(Index));
            }

            var executionStrategy =
                _context.Database
                    .CreateExecutionStrategy();

            try
            {
                return await executionStrategy
                    .ExecuteAsync(
                        async () =>
                        {
                            /*
                             * EnableRetryOnFailure is configured for
                             * this DbContext. Any explicit transaction
                             * must therefore execute inside the EF Core
                             * execution strategy as one retriable unit.
                             *
                             * Clearing tracked state makes a retry start
                             * from the database rather than reusing entity
                             * state left by a failed previous attempt.
                             */
                            _context.ChangeTracker.Clear();

                            await using var transaction =
                                await _context.Database
                                    .BeginTransactionAsync(
                                        IsolationLevel.Serializable,
                                        cancellationToken);

                            var product =
                                await _context.Products
                                    .SingleOrDefaultAsync(
                                        item =>
                                            item.Id == productId,
                                        cancellationToken);

                            if (product is null)
                            {
                                SetErrorMessage(
                                    "The selected product could not be found.");

                                return RedirectToAction(
                                    nameof(Index));
                            }

                            if (!TryResolvePricing(
                                    product,
                                    out _,
                                    out _))
                            {
                                SetErrorMessage(
                                    "This product is not currently available for purchase.");

                                return RedirectToAction(
                                    nameof(Index));
                            }

                            var availableStock =
                                ResolveAvailableStock(
                                    product.Quantity);

                            if (availableStock <= 0)
                            {
                                SetErrorMessage(
                                    "This product is currently out of stock.");

                                return RedirectToAction(
                                    nameof(Index));
                            }

                            /*
                             * We intentionally load all matching rows here.
                             *
                             * The database unique constraint protects
                             * (UserId, ProductId), but keeping this defensive
                             * handling also protects against unexpected legacy
                             * or inconsistent data.
                             */
                            var existingItems =
                                await _context.Carts
                                    .Where(cart =>
                                        cart.UserId == userId
                                        &&
                                        cart.ProductId == productId)
                                    .OrderBy(cart =>
                                        cart.Id)
                                    .ToListAsync(
                                        cancellationToken);

                            long currentQuantity = 0;

                            foreach (var existingItem in existingItems)
                            {
                                currentQuantity +=
                                    Math.Max(
                                        existingItem.Qty,
                                        0);
                            }

                            var requestedQuantity =
                                currentQuantity + quantity;

                            if (requestedQuantity >
                                availableStock)
                            {
                                SetErrorMessage(
                                    $"Only {availableStock} item(s) " +
                                    "are currently available.");

                                return RedirectToAction(
                                    nameof(Index));
                            }

                            if (requestedQuantity >
                                int.MaxValue)
                            {
                                SetErrorMessage(
                                    "The requested quantity is too large.");

                                return RedirectToAction(
                                    nameof(Index));
                            }

                            if (existingItems.Count == 0)
                            {
                                var cartItem =
                                    new Cart
                                    {
                                        UserId =
                                            userId,

                                        ProductId =
                                            product.Id,

                                        Qty =
                                            quantity
                                    };

                                _context.Carts.Add(
                                    cartItem);
                            }
                            else
                            {
                                var primaryItem =
                                    existingItems[0];

                                primaryItem.Qty =
                                    (int)requestedQuantity;

                                if (existingItems.Count > 1)
                                {
                                    _context.Carts.RemoveRange(
                                        existingItems.Skip(1));

                                    _logger.LogWarning(
                                        "Duplicate cart rows were merged " +
                                        "during an add-to-cart operation.");
                                }
                            }

                            await _context.SaveChangesAsync(
                                cancellationToken);

                            await transaction.CommitAsync(
                                cancellationToken);

                            SetSuccessMessage(
                                "Product added to your cart.");

                            _logger.LogInformation(
                                "A product was added to a shopping cart.");

                            return RedirectToAction(
                                nameof(Index));
                        });
            }
            catch (RetryLimitExceededException exception)
            {
                _logger.LogError(
                    exception,
                    "Database retries were exhausted while " +
                    "adding a product to a cart.");

                SetErrorMessage(
                    "We could not update your cart right now. " +
                    "Please try again.");

                return RedirectToAction(
                    nameof(Index));
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "A database error occurred while " +
                    "adding a product to a cart.");

                SetErrorMessage(
                    "We could not update your cart right now. " +
                    "Please try again.");

                return RedirectToAction(
                    nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(
            int cartItemId,
            int quantity,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            if (userId is null)
            {
                return Challenge();
            }

            if (cartItemId <= 0)
            {
                SetErrorMessage(
                    "The selected cart item is invalid.");

                return RedirectToAction(
                    nameof(Index));
            }

            if (quantity <= 0)
            {
                SetErrorMessage(
                    "Quantity must be greater than zero.");

                return RedirectToAction(
                    nameof(Index));
            }

            var cartItem =
                await _context.Carts
                    .Include(cart =>
                        cart.Product)
                    .SingleOrDefaultAsync(
                        cart =>
                            cart.Id == cartItemId
                            &&
                            cart.UserId == userId,
                        cancellationToken);

            if (cartItem is null)
            {
                SetErrorMessage(
                    "The cart item could not be found.");

                return RedirectToAction(
                    nameof(Index));
            }

            if (cartItem.Product is null)
            {
                SetErrorMessage(
                    "This product is no longer available.");

                return RedirectToAction(
                    nameof(Index));
            }

            var availableStock =
                ResolveAvailableStock(
                    cartItem.Product.Quantity);

            if (availableStock <= 0)
            {
                SetErrorMessage(
                    "This product is currently out of stock.");

                return RedirectToAction(
                    nameof(Index));
            }

            if (quantity > availableStock)
            {
                SetErrorMessage(
                    $"Only {availableStock} item(s) " +
                    "are currently available.");

                return RedirectToAction(
                    nameof(Index));
            }

            cartItem.Qty =
                quantity;

            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);

                SetSuccessMessage(
                    "Cart quantity updated.");

                _logger.LogInformation(
                    "A shopping cart quantity was updated.");
            }
            catch (DbUpdateConcurrencyException exception)
            {
                _logger.LogWarning(
                    exception,
                    "A cart item changed before its quantity " +
                    "could be updated.");

                SetErrorMessage(
                    "Your cart changed while it was being updated. " +
                    "Please try again.");
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "A database error occurred while " +
                    "updating a cart quantity.");

                SetErrorMessage(
                    "We could not update your cart right now. " +
                    "Please try again.");
            }

            return RedirectToAction(
                nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(
            int cartItemId,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            if (userId is null)
            {
                return Challenge();
            }

            if (cartItemId <= 0)
            {
                SetErrorMessage(
                    "The selected cart item is invalid.");

                return RedirectToAction(
                    nameof(Index));
            }

            var cartItem =
                await _context.Carts
                    .SingleOrDefaultAsync(
                        cart =>
                            cart.Id == cartItemId
                            &&
                            cart.UserId == userId,
                        cancellationToken);

            if (cartItem is null)
            {
                SetErrorMessage(
                    "The cart item could not be found.");

                return RedirectToAction(
                    nameof(Index));
            }

            _context.Carts.Remove(
                cartItem);

            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);

                SetSuccessMessage(
                    "Product removed from your cart.");

                _logger.LogInformation(
                    "A product was removed from a shopping cart.");
            }
            catch (DbUpdateConcurrencyException exception)
            {
                _logger.LogWarning(
                    exception,
                    "A cart item changed before it " +
                    "could be removed.");

                SetErrorMessage(
                    "Your cart changed while it was being updated. " +
                    "Please try again.");
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "A database error occurred while " +
                    "removing a cart item.");

                SetErrorMessage(
                    "We could not update your cart right now. " +
                    "Please try again.");
            }

            return RedirectToAction(
                nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Clear(
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            if (userId is null)
            {
                return Challenge();
            }

            try
            {
                var deletedRows =
                    await _context.Carts
                        .Where(cart =>
                            cart.UserId == userId)
                        .ExecuteDeleteAsync(
                            cancellationToken);

                if (deletedRows == 0)
                {
                    SetSuccessMessage(
                        "Your cart is already empty.");
                }
                else
                {
                    SetSuccessMessage(
                        "Your cart has been cleared.");

                    _logger.LogInformation(
                        "A shopping cart was cleared.");
                }
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "A database error occurred while " +
                    "clearing a shopping cart.");

                SetErrorMessage(
                    "We could not clear your cart right now. " +
                    "Please try again.");
            }

            return RedirectToAction(
                nameof(Index));
        }

        private string? GetCurrentUserId()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return string.IsNullOrWhiteSpace(userId)
                ? null
                : userId;
        }

        private static bool TryResolvePricing(
            Product product,
            out decimal unitPrice,
            out decimal? originalPrice)
        {
            ArgumentNullException.ThrowIfNull(product);

            unitPrice = 0;
            originalPrice = null;

            if (product.Price <= 0)
            {
                return false;
            }

            var regularPrice =
                product.Price;

            if (!product.Priceafterdiscount.HasValue)
            {
                unitPrice =
                    regularPrice;

                return true;
            }

            var discountedPrice =
                product.Priceafterdiscount.Value;

            if (discountedPrice < 0 ||
                discountedPrice > regularPrice)
            {
                return false;
            }

            if (discountedPrice < regularPrice)
            {
                unitPrice =
                    discountedPrice;

                originalPrice =
                    regularPrice;

                return true;
            }

            unitPrice =
                regularPrice;

            return true;
        }

        private static int ResolveAvailableStock(
            int quantity)
        {
            return Math.Max(
                quantity,
                0);
        }

        private static string? NormalizeOptionalText(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }

        private void SetSuccessMessage(
            string message)
        {
            TempData[SuccessMessageKey] =
                message;
        }

        private void SetErrorMessage(
            string message)
        {
            TempData[ErrorMessageKey] =
                message;
        }
    }
}