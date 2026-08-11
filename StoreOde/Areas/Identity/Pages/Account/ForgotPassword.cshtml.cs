using System.ComponentModel.DataAnnotations;
using System.Text;
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
    public sealed class ForgotPasswordModel : PageModel
    {
        private const int EmailMaxLength = 256;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            ILogger<ForgotPasswordModel> logger)
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

            var email =
                Input.Email.Trim();

            var user =
                await _userManager.FindByEmailAsync(
                    email);

            if (user is null ||
                !await _userManager
                    .IsEmailConfirmedAsync(user))
            {
                /*
                 * Do not reveal whether the account exists
                 * or whether its email has been confirmed.
                 */
                return RedirectToPage(
                    "./ForgotPasswordConfirmation");
            }

            var storedEmail =
                await _userManager.GetEmailAsync(user);

            if (string.IsNullOrWhiteSpace(storedEmail))
            {
                _logger.LogWarning(
                    "A password reset request could not be processed " +
                    "because the stored user email was unavailable.");

                return RedirectToPage(
                    "./ForgotPasswordConfirmation");
            }

            var resetToken =
                await _userManager
                    .GeneratePasswordResetTokenAsync(
                        user);

            var encodedToken =
                WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(
                        resetToken));

            var callbackUrl =
                Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new
                    {
                        area = "Identity",
                        code = encodedToken
                    },
                    protocol: Request.Scheme);

            if (string.IsNullOrWhiteSpace(callbackUrl))
            {
                _logger.LogError(
                    "The password reset callback URL " +
                    "could not be generated.");

                return RedirectToPage(
                    "./ForgotPasswordConfirmation");
            }

            try
            {
                var message =
                    $"""
                    <p>
                        A password reset was requested for your
                        ODEN STORE account.
                    </p>

                    <p>
                        <a href="{callbackUrl}">
                            Reset your password
                        </a>
                    </p>

                    <p>
                        If you did not request a password reset,
                        you can safely ignore this email.
                    </p>
                    """;

                await _emailSender.SendEmailAsync(
                    storedEmail,
                    "Reset your password",
                    message);

                _logger.LogInformation(
                    "A password reset request was processed.");
            }
            catch (Exception exception)
            {
                /*
                 * Do not expose email-delivery failures to the
                 * browser. Different responses for valid accounts
                 * can create an account-enumeration side channel.
                 */
                _logger.LogError(
                    exception,
                    "An error occurred while processing " +
                    "a password reset email.");
            }

            return RedirectToPage(
                "./ForgotPasswordConfirmation");
        }
    }
}