using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using StoreOde.Models;
using StoreOde.ViewModels;

namespace StoreOde.Controllers
{
    public sealed class HomeController : Controller
    {
        private const int MaximumHomeReviews = 10;

        private const string ContactSubmissionRateLimitPolicy =
            "ContactSubmission";

        private const string ReviewSubmissionRateLimitPolicy =
            "ReviewSubmission";

        private readonly SouqcomContext _context;
        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<IdentityUser>
            _userManager;

        public HomeController(
            SouqcomContext context,
            ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(userManager);

            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken)
        {
            var categories =
                await _context.Categories
                    .AsNoTracking()
                    .OrderBy(
                        category =>
                            category.Id)
                    .ToListAsync(
                        cancellationToken);

            /*
             * Only public review information is projected
             * to the home page.
             *
             * Email and Subject are deliberately excluded.
             */
            var reviews =
                await _context.Reviews
                    .AsNoTracking()
                    .OrderByDescending(
                        review =>
                            review.Id)
                    .Take(
                        MaximumHomeReviews)
                    .Select(
                        review =>
                            new Review
                            {
                                Id =
                                    review.Id,

                                Name =
                                    review.Name,

                                Description =
                                    review.Description
                            })
                    .ToListAsync(
                        cancellationToken);

            ViewBag.Reviews =
                reviews;

            return View(
                categories);
        }

        [HttpPost]
        [Authorize]
        [EnableRateLimiting(
            ReviewSubmissionRateLimitPolicy)]
        public async Task<IActionResult> SubmitReview(
            ReviewFormViewModel model,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(model);

            NormalizeReviewForm(
                model);

            /*
             * DataAnnotations validation initially runs before
             * the action executes.
             *
             * Because the values are normalized here, validation
             * is executed again after Trim() so whitespace-only
             * values cannot pass [Required].
             */
            ModelState.Clear();

            TryValidateModel(
                model);

            if (!ModelState.IsValid)
            {
                TempData["ReviewErrorMessage"] =
                    "Please check your review and try again.";

                return RedirectToReviews();
            }

            var currentUser =
                await _userManager.GetUserAsync(
                    User);

            if (currentUser is null)
            {
                _logger.LogWarning(
                    "An authenticated review submission could not " +
                    "be matched to an Identity user.");

                return Challenge();
            }

            var userEmail =
                currentUser.Email?.Trim();

            /*
             * The email is server-owned and comes directly from
             * ASP.NET Core Identity rather than the browser.
             */
            if (string.IsNullOrWhiteSpace(
                    userEmail))
            {
                _logger.LogWarning(
                    "A review submission was rejected because " +
                    "the authenticated Identity user has no email.");

                TempData["ReviewErrorMessage"] =
                    "We could not verify your account email. " +
                    "Please try again later.";

                return RedirectToReviews();
            }

            if (userEmail.Length >
                Review.EmailMaxLength)
            {
                _logger.LogWarning(
                    "A review submission was rejected because " +
                    "the authenticated account email exceeds " +
                    "the Review email storage limit.");

                TempData["ReviewErrorMessage"] =
                    "We could not save your review right now. " +
                    "Please contact support if the problem continues.";

                return RedirectToReviews();
            }

            var review =
                new Review
                {
                    Name =
                        model.Name,

                    Email =
                        userEmail,

                    Subject =
                        model.Subject,

                    Description =
                        model.Description
                };

            try
            {
                _context.Reviews.Add(
                    review);

                await _context.SaveChangesAsync(
                    cancellationToken);

                _logger.LogInformation(
                    "Review {ReviewId} was submitted successfully.",
                    review.Id);

                TempData["ReviewSuccessMessage"] =
                    "Thank you! Your review has been added.";

                return RedirectToReviews();
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "A database error occurred while saving a review.");

                TempData["ReviewErrorMessage"] =
                    "We could not save your review right now. " +
                    "Please try again.";

                return RedirectToReviews();
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected error occurred while " +
                    "processing a review submission.");

                TempData["ReviewErrorMessage"] =
                    "We could not save your review right now. " +
                    "Please try again.";

                return RedirectToReviews();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Products(
            int? catId,
            CancellationToken cancellationToken)
        {
            IQueryable<Product> query =
                _context.Products
                    .AsNoTracking();

            if (catId.HasValue)
            {
                if (catId.Value <= 0)
                {
                    return NotFound();
                }

                var category =
                    await _context.Categories
                        .AsNoTracking()
                        .Where(
                            item =>
                                item.Id ==
                                catId.Value)
                        .Select(
                            item =>
                                new
                                {
                                    item.Id,
                                    item.Name
                                })
                        .SingleOrDefaultAsync(
                            cancellationToken);

                if (category is null)
                {
                    return NotFound();
                }

                ViewBag.CategoryName =
                    category.Name;

                query =
                    query.Where(
                        product =>
                            product.Catid ==
                            category.Id);
            }
            else
            {
                ViewBag.CategoryName =
                    "All Products";
            }

            var products =
                await query
                    .OrderBy(
                        product =>
                            product.Id)
                    .ToListAsync(
                        cancellationToken);

            return View(
                products);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(
                new ContactFormViewModel());
        }

        [HttpPost]
        [ActionName("Savecontact")]
        [EnableRateLimiting(
            ContactSubmissionRateLimitPolicy)]
        public async Task<IActionResult> SaveContact(
            ContactFormViewModel model,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(model);

            NormalizeContactForm(
                model);

            if (!ModelState.IsValid)
            {
                if (IsAjaxRequest())
                {
                    return PlainTextResponse(
                        StatusCodes.Status400BadRequest,
                        "Please correct the form fields and try again.");
                }

                return View(
                    "Contact",
                    model);
            }

            var contactMessage =
                new ContactMessage
                {
                    Name =
                        model.Name,

                    Email =
                        model.Email,

                    Subject =
                        model.Subject,

                    Message =
                        model.Message,

                    CreatedAtUtc =
                        DateTime.UtcNow
                };

            try
            {
                _context.ContactMessages.Add(
                    contactMessage);

                await _context.SaveChangesAsync(
                    cancellationToken);

                _logger.LogInformation(
                    "A contact message was submitted successfully.");

                if (IsAjaxRequest())
                {
                    return Content(
                        "OK",
                        "text/plain");
                }

                TempData["ContactSuccessMessage"] =
                    "Your message has been sent successfully.";

                return RedirectToAction(
                    nameof(Contact));
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "A database error occurred while saving " +
                    "a contact message.");

                return HandleContactSubmissionFailure(
                    model);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected error occurred while processing " +
                    "a contact message.");

                return HandleContactSubmissionFailure(
                    model);
            }
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [IgnoreAntiforgeryToken]
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id
                        ?? HttpContext.TraceIdentifier
                });
        }

        private IActionResult HandleContactSubmissionFailure(
            ContactFormViewModel model)
        {
            const string errorMessage =
                "We could not send your message right now. " +
                "Please try again.";

            if (IsAjaxRequest())
            {
                return PlainTextResponse(
                    StatusCodes.Status500InternalServerError,
                    errorMessage);
            }

            ModelState.AddModelError(
                string.Empty,
                errorMessage);

            return View(
                "Contact",
                model);
        }

        private IActionResult RedirectToReviews()
        {
            return RedirectToAction(
                nameof(Index),
                controllerName: null,
                routeValues: null,
                fragment: "testimonials");
        }

        private static void NormalizeReviewForm(
            ReviewFormViewModel model)
        {
            model.Name =
                model.Name?.Trim()
                ?? string.Empty;

            model.Subject =
                string.IsNullOrWhiteSpace(
                    model.Subject)
                    ? null
                    : model.Subject.Trim();

            model.Description =
                model.Description?.Trim()
                ?? string.Empty;
        }

        private static void NormalizeContactForm(
            ContactFormViewModel model)
        {
            model.Name =
                model.Name?.Trim()
                ?? string.Empty;

            model.Email =
                model.Email?.Trim()
                ?? string.Empty;

            model.Subject =
                model.Subject?.Trim()
                ?? string.Empty;

            model.Message =
                model.Message?.Trim()
                ?? string.Empty;
        }

        private bool IsAjaxRequest()
        {
            if (!Request.Headers.TryGetValue(
                    "X-Requested-With",
                    out var requestedWith))
            {
                return false;
            }

            return string.Equals(
                requestedWith.ToString(),
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase);
        }

        private static ContentResult PlainTextResponse(
            int statusCode,
            string message)
        {
            return new ContentResult
            {
                StatusCode =
                    statusCode,

                Content =
                    message,

                ContentType =
                    "text/plain"
            };
        }
    }
}