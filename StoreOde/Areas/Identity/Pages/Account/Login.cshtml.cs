using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;

namespace StoreOde.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [EnableRateLimiting("IdentityAuthentication")]
    public sealed class LoginModel : PageModel
    {
        private const int EmailMaxLength = 256;
        private const int PasswordMaxLength = 128;

        private const string GenericSignInFailureMessage =
            "Unable to sign in right now. " +
            "Verify your credentials and account status, " +
            "then try again.";

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            ArgumentNullException.ThrowIfNull(signInManager);
            ArgumentNullException.ThrowIfNull(userManager);
            ArgumentNullException.ThrowIfNull(logger);

            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public IList<AuthenticationScheme> ExternalLogins { get; private set; }
            = Array.Empty<AuthenticationScheme>();

        public string ReturnUrl { get; private set; } = "/";

        public sealed class InputModel
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Enter a valid email address.")]
            [StringLength(
                EmailMaxLength,
                ErrorMessage = "Email cannot exceed 256 characters.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            [StringLength(
                PasswordMaxLength,
                ErrorMessage = "Password cannot exceed 128 characters.")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
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

            var user =
                await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                AddGenericSignInFailure();

                return Page();
            }

            var result =
                await _signInManager.PasswordSignInAsync(
                    user,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "A user signed in successfully.");

                return LocalRedirect(ReturnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage(
                    "./LoginWith2fa",
                    new
                    {
                        ReturnUrl,
                        Input.RememberMe
                    });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning(
                    "A user account was temporarily locked out " +
                    "after repeated failed sign-in attempts.");

                AddGenericSignInFailure();

                return Page();
            }

            if (result.IsNotAllowed)
            {
                _logger.LogInformation(
                    "A sign-in attempt was rejected because " +
                    "the account was not allowed to sign in.");

                AddGenericSignInFailure();

                return Page();
            }

            AddGenericSignInFailure();

            return Page();
        }

        private async Task LoadExternalLoginsAsync()
        {
            ExternalLogins =
                (await _signInManager
                    .GetExternalAuthenticationSchemesAsync())
                .ToList();
        }

        private void AddGenericSignInFailure()
        {
            ModelState.AddModelError(
                string.Empty,
                GenericSignInFailureMessage);
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