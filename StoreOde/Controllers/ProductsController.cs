using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreOde.Models;
using StoreOde.ViewModels;

namespace StoreOde.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ProductsController : Controller
    {
        private const string ProductImagesRequestPath =
            "/uploads/products/";

        private readonly SouqcomContext _db;
        private readonly ILogger<ProductsController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(
            SouqcomContext db,
            ILogger<ProductsController> logger,
            IWebHostEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(environment);

            _db = db;
            _logger = logger;
            _environment = environment;
        }

        // GET: Products
        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken)
        {
            var products = await _db.Products
                .AsNoTracking()
                .Include(product => product.Cat)
                .OrderBy(product => product.Id)
                .ToListAsync(cancellationToken);

            return View(products);
        }

        // GET: Products/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(
            int? id,
            CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            var product = await _db.Products
                .AsNoTracking()
                .Include(product => product.Cat)
                .SingleOrDefaultAsync(
                    product => product.Id == id.Value,
                    cancellationToken);

            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [HttpGet]
        public async Task<IActionResult> Create(
            CancellationToken cancellationToken)
        {
            await PopulateCategoriesAsync(
                selectedCategoryId: null,
                cancellationToken);

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        public async Task<IActionResult> Create(
            ProductFormViewModel input,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(input);

            NormalizeProductForm(input);

            await ValidateProductCategoryAsync(
                input.Catid,
                cancellationToken);

            await ValidateProductImageAsync(
                input.ImageFile,
                cancellationToken);

            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync(
                    input.Catid,
                    cancellationToken);

                return View(
                    CreateProductForView(
                        input,
                        DateTime.UtcNow,
                        photo: null));
            }

            string? savedImagePath = null;

            try
            {
                if (input.ImageFile is not null)
                {
                    savedImagePath =
                        await SaveProductImageAsync(
                            input.ImageFile,
                            cancellationToken);
                }
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (IOException exception)
            {
                _logger.LogError(
                    exception,
                    "An I/O error occurred while saving a new product image.");

                ModelState.AddModelError(
                    nameof(ProductFormViewModel.ImageFile),
                    "The product image could not be saved. Please try again.");

                await PopulateCategoriesAsync(
                    input.Catid,
                    cancellationToken);

                return View(
                    CreateProductForView(
                        input,
                        DateTime.UtcNow,
                        photo: null));
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogError(
                    exception,
                    "Access to the product image storage directory was denied.");

                ModelState.AddModelError(
                    nameof(ProductFormViewModel.ImageFile),
                    "The product image could not be saved. Please try again.");

                await PopulateCategoriesAsync(
                    input.Catid,
                    cancellationToken);

                return View(
                    CreateProductForView(
                        input,
                        DateTime.UtcNow,
                        photo: null));
            }

            var product = new Product
            {
                EntryDate = DateTime.UtcNow,
                Photo = savedImagePath
            };

            ApplyProductForm(
                input,
                product);

            try
            {
                _db.Products.Add(product);

                await _db.SaveChangesAsync(
                    cancellationToken);

                _logger.LogInformation(
                    "Product {ProductId} with name {ProductName} was created.",
                    product.Id,
                    product.Name);

                return RedirectToAction(
                    nameof(Index));
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                DeleteManagedProductImage(
                    savedImagePath);

                throw;
            }
            catch (DbUpdateException exception)
            {
                DeleteManagedProductImage(
                    savedImagePath);

                _logger.LogError(
                    exception,
                    "A database error occurred while creating product {ProductName}.",
                    product.Name);

                ModelState.AddModelError(
                    string.Empty,
                    "The product could not be created. Please try again.");

                await PopulateCategoriesAsync(
                    input.Catid,
                    cancellationToken);

                return View(
                    CreateProductForView(
                        input,
                        product.EntryDate,
                        photo: null));
            }
        }

        // GET: Products/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(
            int? id,
            CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            var product = await _db.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    product => product.Id == id.Value,
                    cancellationToken);

            if (product is null)
            {
                return NotFound();
            }

            await PopulateCategoriesAsync(
                product.Catid,
                cancellationToken);

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            ProductFormViewModel input,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (id != input.Id)
            {
                return BadRequest();
            }

            var product = await _db.Products
                .SingleOrDefaultAsync(
                    product => product.Id == id,
                    cancellationToken);

            if (product is null)
            {
                return NotFound();
            }

            var originalPhoto =
                product.Photo;

            NormalizeProductForm(input);

            await ValidateProductCategoryAsync(
                input.Catid,
                cancellationToken);

            await ValidateProductImageAsync(
                input.ImageFile,
                cancellationToken);

            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync(
                    input.Catid,
                    cancellationToken);

                return View(
                    CreateProductForView(
                        input,
                        product.EntryDate,
                        originalPhoto));
            }

            string? savedImagePath = null;

            try
            {
                if (input.ImageFile is not null)
                {
                    savedImagePath =
                        await SaveProductImageAsync(
                            input.ImageFile,
                            cancellationToken);
                }
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (IOException exception)
            {
                _logger.LogError(
                    exception,
                    "An I/O error occurred while saving a replacement image for product {ProductId}.",
                    id);

                ModelState.AddModelError(
                    nameof(ProductFormViewModel.ImageFile),
                    "The product image could not be saved. Please try again.");

                await PopulateCategoriesAsync(
                    input.Catid,
                    cancellationToken);

                return View(
                    CreateProductForView(
                        input,
                        product.EntryDate,
                        originalPhoto));
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogError(
                    exception,
                    "Access to the product image storage directory was denied while updating product {ProductId}.",
                    id);

                ModelState.AddModelError(
                    nameof(ProductFormViewModel.ImageFile),
                    "The product image could not be saved. Please try again.");

                await PopulateCategoriesAsync(
                    input.Catid,
                    cancellationToken);

                return View(
                    CreateProductForView(
                        input,
                        product.EntryDate,
                        originalPhoto));
            }

            ApplyProductForm(
                input,
                product);

            if (savedImagePath is not null)
            {
                product.Photo =
                    savedImagePath;
            }

            try
            {
                await _db.SaveChangesAsync(
                    cancellationToken);

                if (savedImagePath is not null &&
                    !string.Equals(
                        originalPhoto,
                        savedImagePath,
                        StringComparison.OrdinalIgnoreCase))
                {
                    DeleteManagedProductImage(
                        originalPhoto);
                }

                _logger.LogInformation(
                    "Product {ProductId} with name {ProductName} was updated.",
                    product.Id,
                    product.Name);

                return RedirectToAction(
                    nameof(Index));
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                DeleteManagedProductImage(
                    savedImagePath);

                throw;
            }
            catch (DbUpdateConcurrencyException exception)
            {
                DeleteManagedProductImage(
                    savedImagePath);

                if (!await ProductExistsAsync(
                        id,
                        cancellationToken))
                {
                    return NotFound();
                }

                _logger.LogError(
                    exception,
                    "A concurrency error occurred while updating product {ProductId}.",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "The product was modified by another operation. Reload the page and try again.");
            }
            catch (DbUpdateException exception)
            {
                DeleteManagedProductImage(
                    savedImagePath);

                _logger.LogError(
                    exception,
                    "A database error occurred while updating product {ProductId}.",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "The product could not be updated. Please try again.");
            }

            await PopulateCategoriesAsync(
                input.Catid,
                cancellationToken);

            return View(
                CreateProductForView(
                    input,
                    product.EntryDate,
                    originalPhoto));
        }

        // GET: Products/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(
            int? id,
            CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            var product = await _db.Products
                .AsNoTracking()
                .Include(product => product.Cat)
                .SingleOrDefaultAsync(
                    product => product.Id == id.Value,
                    cancellationToken);

            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(
            int id,
            CancellationToken cancellationToken)
        {
            var product = await _db.Products
                .SingleOrDefaultAsync(
                    product => product.Id == id,
                    cancellationToken);

            if (product is null)
            {
                return NotFound();
            }

            var isReferencedByCart = await _db.Carts
                .AsNoTracking()
                .AnyAsync(
                    cart => cart.ProductId == id,
                    cancellationToken);

            if (isReferencedByCart)
            {
                _logger.LogWarning(
                    "Deletion of product {ProductId} was prevented because the product is referenced by one or more carts.",
                    id);

                return Conflict(
                    "This product cannot be deleted because it is currently referenced by one or more shopping carts.");
            }

            var productPhoto =
                product.Photo;

            try
            {
                _db.Products.Remove(product);

                await _db.SaveChangesAsync(
                    cancellationToken);

                DeleteManagedProductImage(
                    productPhoto);

                _logger.LogInformation(
                    "Product {ProductId} with name {ProductName} was deleted.",
                    product.Id,
                    product.Name);

                return RedirectToAction(
                    nameof(Index));
            }
            catch (DbUpdateConcurrencyException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Product {ProductId} was deleted by another operation before this request completed.",
                    id);

                return NotFound();
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "A database error occurred while deleting product {ProductId}.",
                    id);

                return Problem(
                    title: "Unable to delete product",
                    detail:
                        "The product could not be deleted because it may still be referenced by other data.",
                    statusCode:
                        StatusCodes.Status409Conflict);
            }
        }

        private async Task PopulateCategoriesAsync(
            int? selectedCategoryId,
            CancellationToken cancellationToken)
        {
            var categories = await _db.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .ToListAsync(cancellationToken);

            ViewData["Catid"] = new SelectList(
                categories,
                nameof(Category.Id),
                nameof(Category.Name),
                selectedCategoryId);
        }

        private async Task ValidateProductCategoryAsync(
            int categoryId,
            CancellationToken cancellationToken)
        {
            if (categoryId <= 0)
            {
                return;
            }

            var categoryExists = await _db.Categories
                .AsNoTracking()
                .AnyAsync(
                    category => category.Id == categoryId,
                    cancellationToken);

            if (!categoryExists)
            {
                ModelState.AddModelError(
                    nameof(ProductFormViewModel.Catid),
                    "The selected category does not exist.");
            }
        }

        private async Task ValidateProductImageAsync(
            IFormFile? imageFile,
            CancellationToken cancellationToken)
        {
            if (imageFile is null ||
                imageFile.Length <= 0 ||
                imageFile.Length >
                    ProductFormViewModel.ImageMaxFileSizeBytes)
            {
                return;
            }

            var extension =
                Path.GetExtension(
                    imageFile.FileName);

            if (string.IsNullOrWhiteSpace(extension))
            {
                return;
            }

            var header =
                new byte[12];

            await using var stream =
                imageFile.OpenReadStream();

            var bytesRead = 0;

            while (bytesRead < header.Length)
            {
                var read =
                    await stream.ReadAsync(
                        header.AsMemory(
                            bytesRead,
                            header.Length - bytesRead),
                        cancellationToken);

                if (read == 0)
                {
                    break;
                }

                bytesRead += read;
            }

            var isValid =
                extension.ToLowerInvariant() switch
                {
                    ".jpg" or ".jpeg" =>
                        IsJpeg(header, bytesRead),

                    ".png" =>
                        IsPng(header, bytesRead),

                    ".webp" =>
                        IsWebP(header, bytesRead),

                    _ =>
                        false
                };

            if (!isValid)
            {
                ModelState.AddModelError(
                    nameof(ProductFormViewModel.ImageFile),
                    "The selected file does not contain a valid JPG, PNG, or WEBP image.");
            }
        }

        private async Task<string> SaveProductImageAsync(
            IFormFile imageFile,
            CancellationToken cancellationToken)
        {
            var extension =
                Path.GetExtension(
                        imageFile.FileName)
                    .ToLowerInvariant();

            var fileName =
                $"{Guid.NewGuid():N}{extension}";

            var uploadDirectory =
                Path.Combine(
                    GetWebRootPath(),
                    "uploads",
                    "products");

            Directory.CreateDirectory(
                uploadDirectory);

            var physicalPath =
                Path.Combine(
                    uploadDirectory,
                    fileName);

            await using var outputStream =
                new FileStream(
                    physicalPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true);

            await imageFile.CopyToAsync(
                outputStream,
                cancellationToken);

            return $"{ProductImagesRequestPath}{fileName}";
        }

        private void DeleteManagedProductImage(
            string? photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath) ||
                !photoPath.StartsWith(
                    ProductImagesRequestPath,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var fileName =
                Path.GetFileName(
                    photoPath);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            var physicalPath =
                Path.Combine(
                    GetWebRootPath(),
                    "uploads",
                    "products",
                    fileName);

            try
            {
                if (System.IO.File.Exists(
                    physicalPath))
                {
                    System.IO.File.Delete(
                        physicalPath);
                }
            }
            catch (IOException exception)
            {
                _logger.LogWarning(
                    exception,
                    "The product image file {PhotoPath} could not be deleted.",
                    photoPath);
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Access was denied while deleting product image file {PhotoPath}.",
                    photoPath);
            }
        }

        private string GetWebRootPath()
        {
            if (!string.IsNullOrWhiteSpace(
                    _environment.WebRootPath))
            {
                return _environment.WebRootPath;
            }

            return Path.Combine(
                _environment.ContentRootPath,
                "wwwroot");
        }

        private async Task<bool> ProductExistsAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _db.Products
                .AsNoTracking()
                .AnyAsync(
                    product => product.Id == id,
                    cancellationToken);
        }

        private static void NormalizeProductForm(
            ProductFormViewModel input)
        {
            input.Name =
                input.Name?.Trim()
                ?? string.Empty;

            input.Description =
                input.Description?.Trim();

            input.Type =
                input.Type?.Trim();

            input.SupplierName =
                input.SupplierName?.Trim();

            input.ReviewUrl =
                input.ReviewUrl?.Trim();
        }

        private static void ApplyProductForm(
            ProductFormViewModel input,
            Product product)
        {
            product.Name =
                input.Name;

            product.Description =
                input.Description;

            product.Price =
                input.Price;

            product.Catid =
                input.Catid;

            product.Type =
                input.Type;

            product.SupplierName =
                input.SupplierName;

            product.ReviewUrl =
                input.ReviewUrl;

            product.Quantity =
                input.Quantity;

            product.Priceafterdiscount =
                input.Priceafterdiscount;
        }

        private static Product CreateProductForView(
            ProductFormViewModel input,
            DateTime? entryDate,
            string? photo)
        {
            var product = new Product
            {
                Id = input.Id,
                EntryDate = entryDate,
                Photo = photo
            };

            ApplyProductForm(
                input,
                product);

            return product;
        }

        private static bool IsJpeg(
            byte[] header,
            int length)
        {
            return length >= 3 &&
                   header[0] == 0xFF &&
                   header[1] == 0xD8 &&
                   header[2] == 0xFF;
        }

        private static bool IsPng(
            byte[] header,
            int length)
        {
            return length >= 8 &&
                   header[0] == 0x89 &&
                   header[1] == 0x50 &&
                   header[2] == 0x4E &&
                   header[3] == 0x47 &&
                   header[4] == 0x0D &&
                   header[5] == 0x0A &&
                   header[6] == 0x1A &&
                   header[7] == 0x0A;
        }

        private static bool IsWebP(
            byte[] header,
            int length)
        {
            return length >= 12 &&
                   header[0] == (byte)'R' &&
                   header[1] == (byte)'I' &&
                   header[2] == (byte)'F' &&
                   header[3] == (byte)'F' &&
                   header[8] == (byte)'W' &&
                   header[9] == (byte)'E' &&
                   header[10] == (byte)'B' &&
                   header[11] == (byte)'P';
        }
    }
}