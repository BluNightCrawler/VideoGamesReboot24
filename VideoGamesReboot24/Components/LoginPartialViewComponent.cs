using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoGamesReboot24.Models;

namespace VideoGamesReboot24.Components
{
    public class LoginPartialViewComponent : ViewComponent
    {
        UserManager<AppUser> userManager;

        public LoginPartialViewComponent(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            AppUser appUser = await userManager.GetUserAsync(HttpContext.User);
            return View(appUser);
        }
    }
}
