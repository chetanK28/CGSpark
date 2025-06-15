using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    [HttpPost]
    public IActionResult SignOut()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            "Cookies", "OpenIdConnect");
    }
}