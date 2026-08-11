using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreOde.Models;

namespace StoreOde.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class CategoriesController : Controller
    {
        private readonly SouqcomContext _db;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            SouqcomContext db,
            ILogger<CategoriesController> logger)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(logger);

            _db = db;
            _logger = logger;
        }

        // GET: Categories
        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken)
        {
            var categories = await _db.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .ToListAsync(cancellationToken);

            return View(categories);
        }

        // GET: Categories/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(
            int? id,
            CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            var category = await _db.Categories
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    category => category.Id == id.Value,
                    cancellationToken);

            if (category is null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("Name,IconClass,Description")] Category category,
            CancellationToken cancellationToken)
        {
            NormalizeCategory(category);
            ValidateCategory(category);

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                _db.Categories.Add(category);

                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Category {CategoryId} with name {CategoryName} was created.",
                    category.Id,
                    category.Name);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "A database error occurred while creating category {CategoryName}.",
                    category.Name);

                ModelState.AddModelError(
                    string.Empty,
                    "The category could not be created. Please try again.");

                return View(category);
            }
        }

        // GET: Categories/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(
            int? id,
            CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            var category = await _db.Categories
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    category => category.Id == id.Value,
                    cancellationToken);

            if (category is null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,IconClass,Description")] Category input,
            CancellationToken cancellationToken)
        {
            if (id != input.Id)
            {
                return BadRequest();
            }

            NormalizeCategory(input);
            ValidateCategory(input);

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var category = await _db.Categories
                .SingleOrDefaultAsync(
                    category => category.Id == id,
                    cancellationToken);

            if (category is null)
            {
                return NotFound();
            }

            category.Name = input.Name;
            category.IconClass = input.IconClass;
            category.Description = input.Description;

            try
            {
                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Category {CategoryId} was updated.",
                    category.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException exception)
            {
                if (!await CategoryExistsAsync(
                        id,
                        cancellationToken))
                {
                    return NotFound();
                }

                _logger.LogError(
                    exception,
                    "A concurrency error occurred while updating category {CategoryId}.",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "The category was modified by another operation. Please reload the page and try again.");

                return View(input);
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "A database error occurred while updating category {CategoryId}.",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "The category could not be updated. Please try again.");

                return View(input);
            }
        }

        // GET: Categories/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(
            int? id,
            CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            var category = await _db.Categories
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    category => category.Id == id.Value,
                    cancellationToken);

            if (category is null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(
            int id,
            CancellationToken cancellationToken)
        {
            var category = await _db.Categories
                .SingleOrDefaultAsync(
                    category => category.Id == id,
                    cancellationToken);

            if (category is null)
            {
                return NotFound();
            }

            var hasProducts = await _db.Products
                .AsNoTracking()
                .AnyAsync(
                    product => product.Catid == id,
                    cancellationToken);

            if (hasProducts)
            {
                _logger.LogWarning(
                    "Deletion of category {CategoryId} was prevented because products are associated with it.",
                    id);

                return Conflict(
                    "This category cannot be deleted because one or more products are associated with it.");
            }

            try
            {
                _db.Categories.Remove(category);

                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Category {CategoryId} with name {CategoryName} was deleted.",
                    category.Id,
                    category.Name);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Category {CategoryId} was deleted by another operation before this request completed.",
                    id);

                return NotFound();
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "A database error occurred while deleting category {CategoryId}.",
                    id);

                return Problem(
                    title: "Unable to delete category",
                    detail:
                        "The category could not be deleted because it may still be referenced by other data.",
                    statusCode: StatusCodes.Status409Conflict);
            }
        }

        private async Task<bool> CategoryExistsAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _db.Categories
                .AsNoTracking()
                .AnyAsync(
                    category => category.Id == id,
                    cancellationToken);
        }

        private void ValidateCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError(
                    nameof(Category.Name),
                    "Category name is required.");
            }
        }

        private static void NormalizeCategory(Category category)
        {
            category.Name =
                category.Name?.Trim();

            category.IconClass =
                category.IconClass?.Trim();

            category.Description =
                category.Description?.Trim();
        }
    }
}