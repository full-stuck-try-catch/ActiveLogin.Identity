using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    public IActionResult OnPostLogin()
    {
        return Challenge(new AuthenticationProperties { RedirectUri = Url.Page("/Index") }, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
