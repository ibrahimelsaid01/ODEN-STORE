using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;

namespace StoreOde.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [EnableRateLimiting("IdentityEmail")]
    public sealed class ResendEmailConfirmationModel : PageModel
    {
        private const int EmailMaxLength = 256;

        private const string GenericStatusMessage =
            "If the email address is eligible, " +
            "you should receive a confirmation email shortly.";

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ResendEmailConfirmationModel> _logger;

        public ResendEmailConfirmationModel(
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            ILogger<ResendEmailConfirmationModel> logger)
        {
            ArgumentNullException.ThrowIfNull(userManager);
            ArgumentNullException.ThrowIfNull(emailSender);
            ArgumentNullException.ThrowIfNull(logger);

            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public sealed class InputModel
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Enter a valid email address.")]
            [StringLength(
                EmailMaxLength,
                ErrorMessage =
                    "Email cannot exceed 256 characters.")]
            public string Email { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var email = Input.Email.Trim();

            var user =
                await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return RedirectWithGenericStatus();
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return RedirectWithGenericStatus();
            }

            await TrySendConfirmationEmailAsync(user);

            return RedirectWithGenericStatus();
        }

        private async Task TrySendConfirmationEmailAsync(
            IdentityUser user)
        {
            try
            {
                var userId =
                    await _userManager.GetUserIdAsync(user);

                var email =
                    await _userManager.GetEmailAsync(user);

                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning(
                        "A confirmation email could not be sent " +
                        "because the stored user email was unavailable.");

                    return;
                }

                var confirmationToken =
                    await _userManager
                        .GenerateEmailConfirmationTokenAsync(user);

                var encodedToken =
                    WebEncoders.Base64UrlEncode(
                        Encoding.UTF8.GetBytes(
                            confirmationToken));

                var callbackUrl =
                    Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new
                        {
                            area = "Identity",
                            userId,
                            code = encodedToken
                        },
                        protocol: Request.Scheme);

                if (string.IsNullOrWhiteSpace(callbackUrl))
                {
                    _logger.LogError(
                        "The email confirmation callback URL " +
                        "could not be generated.");

                    return;
                }

                var encodedCallbackUrl =
                    HtmlEncoder.Default.Encode(
                        callbackUrl);

                var message =
                    $"""
                    <p>
                        Please confirm your ODEN STORE account.
                    </p>

                    <p>
                        <a href="{encodedCallbackUrl}">
                            Confirm your email
                        </a>
                    </p>

                    <p>
                        If you did not request this email,
                        you can safely ignore it.
                    </p>
                    """;

                await _emailSender.SendEmailAsync(
                    email,
                    "Confirm your email",
                    message);

                _logger.LogInformation(
                    "A confirmation email resend request " +
                    "was processed successfully.");
            }
            catch (Exception exception)
            {
                /*
                 * Do not expose SMTP or provider failures to
                 * the browser. A different response for a valid
                 * account could create an account-enumeration
                 * side channel.
                 */
                _logger.LogError(
                    exception,
                    "An error occurred while processing " +
                    "an email confirmation resend request.");
            }
        }

        private IActionResult RedirectWithGenericStatus()
        {
            StatusMessage =
                GenericStatusMessage;

            return RedirectToPage();
        }
    }
}