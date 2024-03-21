using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoGamesReboot24.Infrastructure;
using VideoGamesReboot24.Models;
using VideoGamesReboot24.Models.ViewModels;

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
            Cart cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            int cartItemCount = cart.Lines.Count;
            AppUser appUser = await userManager.GetUserAsync(HttpContext.User);
            AppUserWithCartInfo appUserWithCartInfo = new AppUserWithCartInfo { AppUser = appUser , CartItemCount = cartItemCount};
            return View(appUserWithCartInfo);
        }
    }
}
