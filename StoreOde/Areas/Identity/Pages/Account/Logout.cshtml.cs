using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StoreOde.Areas.Identity.Pages.Account
{
    [Authorize]
    public sealed class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(
            SignInManager<IdentityUser> signInManager,
            ILogger<LogoutModel> logger)
        {
            ArgumentNullException.ThrowIfNull(signInManager);
            ArgumentNullException.ThrowIfNull(logger);

            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            /*
             * Logout is intentionally POST-only.
             *
             * A GET request must never change the authentication state.
             */
            return LocalRedirect(
                Url.Content("~/"));
        }

        public async Task<IActionResult> OnPostAsync(
            string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation(
                "An authenticated user signed out successfully.");

            return LocalRedirect(
                NormalizeReturnUrl(returnUrl));
        }

        private string NormalizeReturnUrl(
            string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return Url.Content("~/");
            }

            return Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : Url.Content("~/");
        }
    }
}