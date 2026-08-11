using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;

namespace StoreOde.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [EnableRateLimiting("IdentityPasswordReset")]
    public sealed class ResetPasswordModel : PageModel
    {
        private const int EmailMaxLength = 256;
        private const int PasswordMinLength = 10;
        private const int PasswordMaxLength = 128;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(
            UserManager<IdentityUser> userManager,
            ILogger<ResetPasswordModel> logger)
        {
            ArgumentNullException.ThrowIfNull(userManager);
            ArgumentNullException.ThrowIfNull(logger);

            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public bool InvalidResetLink { get; private set; }

        public sealed class InputModel
        {
            [Required]
            public string Code { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Enter a valid email address.")]
            [StringLength(
                EmailMaxLength,
                ErrorMessage = "Email cannot exceed 256 characters.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(
                PasswordMaxLength,
                MinimumLength = PasswordMinLength,
                ErrorMessage =
                    "Password must be between 10 and 128 characters long.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password confirmation is required.")]
            [DataType(DataType.Password)]
            [Compare(
                nameof(Password),
                ErrorMessage =
                    "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet(
            string? code = null)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                InvalidResetLink = true;

                return Page();
            }

            Input = new InputModel
            {
                Code = code
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            /*
             * Input.Code is rendered as a hidden field.
             *
             * Treat a missing code as an invalid reset link
             * instead of returning the reset form with a hidden
             * validation error that the user cannot act on.
             */
            if (string.IsNullOrWhiteSpace(Input.Code))
            {
                InvalidResetLink = true;

                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var email = Input.Email.Trim();

            var user =
                await _userManager.FindByEmailAsync(email);

            /*
             * Keep the observable response consistent with the
             * invalid-token path.
             *
             * Returning the confirmation page for an unknown email
             * while showing an invalid-token page for an existing
             * but non-matching account can disclose account state.
             */
            if (user is null)
            {
                InvalidResetLink = true;

                return Page();
            }

            string resetToken;

            try
            {
                var decodedCode =
                    WebEncoders.Base64UrlDecode(
                        Input.Code);

                resetToken =
                    Encoding.UTF8.GetString(
                        decodedCode);
            }
            catch (FormatException exception)
            {
                _logger.LogWarning(
                    exception,
                    "An invalid password reset token encoding was received.");

                InvalidResetLink = true;

                return Page();
            }

            var result =
                await _userManager.ResetPasswordAsync(
                    user,
                    resetToken,
                    Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "A user password was reset successfully.");

                return RedirectToPage(
                    "./ResetPasswordConfirmation");
            }

            AddIdentityErrors(result);

            return Page();
        }

        private void AddIdentityErrors(
            IdentityResult result)
        {
            var invalidToken =
                result.Errors.Any(
                    error =>
                        string.Equals(
                            error.Code,
                            nameof(
                                IdentityErrorDescriber.InvalidToken),
                            StringComparison.OrdinalIgnoreCase));

            if (invalidToken)
            {
                InvalidResetLink = true;

                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }
        }
    }
}