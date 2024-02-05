using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using VideoGamesReboot24.Models;
using VideoGamesReboot24.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VideoGamesReboot24.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        //public IActionResult Index()
        //{
        //    return View("Login");
        //}
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser(UserCredentials credentials)
        {
            if (!ModelState.IsValid)
            {
                return View(credentials);
            }
            IdentityUser user = await userManager.FindByNameAsync(credentials.UserName);
            if (user == null) {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View("Login");
            }
            var result = await signInManager.PasswordSignInAsync(user, credentials.Password, credentials.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View("Login");
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegistration registration)
        {
            if (!ModelState.IsValid) return View(registration);

            IdentityUser user = await userManager.FindByNameAsync(registration.UserName);

            if (user != null) {
                ModelState.AddModelError("", "Username Taken");
                return View(registration); 
            }

            user = new IdentityUser(registration.UserName);
            user.Email = registration.Email;

            var result = await userManager.CreateAsync(user, registration.Password);

            if (result.Succeeded)
            {
                ViewBag.SuccessfullyRegistered = true;
                return View("Login");
            }
            else
            {
                if (result != null)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View(registration);
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create user");
                    return View(registration);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
