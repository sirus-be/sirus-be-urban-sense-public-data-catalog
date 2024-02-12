using System.Threading.Tasks;
using Core.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DataCatalog.Portal.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly TokenProvider tokenProvider;

        public LogoutModel(TokenProvider tokenProvider)
        {
            this.tokenProvider = tokenProvider;
        }
        public async Task OnGet()
        {
            var authProps = new AuthenticationProperties
            {
                RedirectUri = Url.Content("~/")
            };

            tokenProvider.Remove();
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc", authProps);
        }
    }
}
