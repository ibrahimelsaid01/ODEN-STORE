using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
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
    public sealed class RegisterModel : PageModel
    {
        private const int EmailMaxLength = 256;
        private const int PasswordMinLength = 10;
        private const int PasswordMaxLength = 128;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUserStore<IdentityUser> userStore,
            IEmailSender emailSender,
            ILogger<RegisterModel> logger)
        {
            ArgumentNullException.ThrowIfNull(userManager);
            ArgumentNullException.ThrowIfNull(signInManager);
            ArgumentNullException.ThrowIfNull(userStore);
            ArgumentNullException.ThrowIfNull(emailSender);
            ArgumentNullException.ThrowIfNull(logger);

            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
            _emailSender = emailSender;
            _logger = logger;

            _emailStore = GetEmailStore();
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string ReturnUrl { get; private set; } = "/";

        public IList<AuthenticationScheme> ExternalLogins { get; private set; }
            = Array.Empty<AuthenticationScheme>();

        public sealed class InputModel
        {
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

        public async Task OnGetAsync(
            string? returnUrl = null)
        {
            ReturnUrl = NormalizeReturnUrl(returnUrl);

            await LoadExternalLoginsAsync();
        }

        public async Task<IActionResult> OnPostAsync(
            string? returnUrl = null)
        {
            ReturnUrl = NormalizeReturnUrl(returnUrl);

            await LoadExternalLoginsAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var email = Input.Email.Trim();

            var user = CreateUser();

            await _userStore.SetUserNameAsync(
                user,
                email,
                CancellationToken.None);

            await _emailStore.SetEmailAsync(
                user,
                email,
                CancellationToken.None);

            var result =
                await _userManager.CreateAsync(
                    user,
                    Input.Password);

            if (!result.Succeeded)
            {
                AddRegistrationErrors(result);

                return Page();
            }

            _logger.LogInformation(
                "A new user account was created.");

            await TrySendConfirmationEmailAsync(
                user,
                ReturnUrl);

            if (_userManager.Options.SignIn.RequireConfirmedAccount ||
                _userManager.Options.SignIn.RequireConfirmedEmail)
            {
                return RedirectToPage(
                    "RegisterConfirmation",
                    new
                    {
                        returnUrl = ReturnUrl
                    });
            }

            await _signInManager.SignInAsync(
                user,
                isPersistent: false);

            return LocalRedirect(ReturnUrl);
        }

        private async Task TrySendConfirmationEmailAsync(
            IdentityUser user,
            string returnUrl)
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
                        "A registration confirmation email could not " +
                        "be sent because the stored email was unavailable.");

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
                            code = encodedToken,
                            returnUrl
                        },
                        protocol: Request.Scheme);

                if (string.IsNullOrWhiteSpace(callbackUrl))
                {
                    _logger.LogError(
                        "The registration confirmation callback URL " +
                        "could not be generated.");

                    return;
                }

                var encodedCallbackUrl =
                    HtmlEncoder.Default.Encode(
                        callbackUrl);

                var message =
                    $"""
                    <p>
                        Thank you for creating an ODEN STORE account.
                    </p>

                    <p>
                        Please confirm your email address by clicking
                        the link below:
                    </p>

                    <p>
                        <a href="{encodedCallbackUrl}">
                            Confirm your email
                        </a>
                    </p>

                    <p>
                        If you did not create this account,
                        you can safely ignore this email.
                    </p>
                    """;

                await _emailSender.SendEmailAsync(
                    email,
                    "Confirm your email",
                    message);

                _logger.LogInformation(
                    "A registration confirmation email was processed.");
            }
            catch (Exception exception)
            {
                /*
                 * Account creation has already succeeded.
                 *
                 * Do not remove the user merely because the external
                 * email provider failed. The user can request another
                 * confirmation email through the resend flow.
                 */
                _logger.LogError(
                    exception,
                    "An error occurred while processing " +
                    "a registration confirmation email.");
            }
        }

        private void AddRegistrationErrors(
            IdentityResult result)
        {
            var duplicateAccount =
                result.Errors.Any(
                    error =>
                        string.Equals(
                            error.Code,
                            nameof(
                                IdentityErrorDescriber.DuplicateEmail),
                            StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(
                            error.Code,
                            nameof(
                                IdentityErrorDescriber.DuplicateUserName),
                            StringComparison.OrdinalIgnoreCase));

            if (duplicateAccount)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the account with the provided details.");

                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }
        }

        private async Task LoadExternalLoginsAsync()
        {
            ExternalLogins =
                (await _signInManager
                    .GetExternalAuthenticationSchemesAsync())
                .ToList();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Cannot create an instance of " +
                    $"'{nameof(IdentityUser)}'.",
                    exception);
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException(
                    "The configured Identity user store " +
                    "does not support email addresses.");
            }

            return (IUserEmailStore<IdentityUser>)_userStore;
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