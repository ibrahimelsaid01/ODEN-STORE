using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StoreOde.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public sealed class RegisterConfirmationModel : PageModel
    {
        public string ReturnUrl { get; private set; } = "/";

        public void OnGet(
            string? returnUrl = null)
        {
            ReturnUrl =
                NormalizeReturnUrl(returnUrl);
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