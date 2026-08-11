using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace StoreOde.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public sealed class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ConfirmEmailModel> _logger;

        public ConfirmEmailModel(
            UserManager<IdentityUser> userManager,
            ILogger<ConfirmEmailModel> logger)
        {
            ArgumentNullException.ThrowIfNull(userManager);
            ArgumentNullException.ThrowIfNull(logger);

            _userManager = userManager;
            _logger = logger;
        }

        public bool Succeeded { get; private set; }

        public string StatusMessage { get; private set; }
            = string.Empty;

        public string ReturnUrl { get; private set; } = "/";

        public async Task<IActionResult> OnGetAsync(
            string? userId,
            string? code,
            string? returnUrl = null)
        {
            ReturnUrl = NormalizeReturnUrl(returnUrl);

            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(code))
            {
                SetFailure();

                return Page();
            }

            var user =
                await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                SetFailure();

                return Page();
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                Succeeded = true;

                StatusMessage =
                    "Your email has already been confirmed.";

                return Page();
            }

            string confirmationToken;

            try
            {
                var decodedCode =
                    WebEncoders.Base64UrlDecode(code);

                confirmationToken =
                    Encoding.UTF8.GetString(decodedCode);
            }
            catch (FormatException exception)
            {
                _logger.LogWarning(
                    exception,
                    "An invalid email confirmation token encoding was received.");

                SetFailure();

                return Page();
            }

            var result =
                await _userManager.ConfirmEmailAsync(
                    user,
                    confirmationToken);

            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "An email confirmation attempt failed.");

                SetFailure();

                return Page();
            }

            Succeeded = true;

            StatusMessage =
                "Thank you for confirming your email.";

            _logger.LogInformation(
                "A user successfully confirmed their email address.");

            return Page();
        }

        private void SetFailure()
        {
            Succeeded = false;

            StatusMessage =
                "We could not confirm your email. " +
                "The confirmation link may be invalid or expired.";
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