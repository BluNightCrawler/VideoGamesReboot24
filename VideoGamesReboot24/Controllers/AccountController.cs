using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using VideoGamesReboot24.Models;
using VideoGamesReboot24.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace VideoGamesReboot24.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly GameStoreDbContext gameStoreDbContext;
        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, GameStoreDbContext gameStoreDbContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.gameStoreDbContext = gameStoreDbContext;
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
            AppUser user = await userManager.FindByNameAsync(credentials.UserName);
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

            AppUser user = await userManager.FindByNameAsync(registration.UserName);

            if (user != null) {
                ModelState.AddModelError("", "Username Taken");
                return View(registration); 
            }

            user = new AppUser(registration.UserName);
            user.Email = registration.Email;
            user.ImagePath = "~/Images/placeholder.jpg";

            var result = await userManager.CreateAsync(user, registration.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
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
        [Authorize(Policy = "LoginRestricted")]
        public async Task<IActionResult> Manage()
        {
            AppUser currentUser = await userManager.FindByNameAsync(User.Identity.Name);
            UserAccount userAccount = new UserAccount()
            {
                Email = currentUser.Email,
                UserName = currentUser.UserName,
                ImagePath = currentUser.ImagePath,
                IsAdmin = (await userManager.GetRolesAsync(currentUser)).Contains("Admin")
            };
            return View(userAccount);
        }

        [HttpPost]
        [Authorize(Policy = "LoginRestricted")]
        public async Task<IActionResult> Manage(UserAccount account)
        {
            AppUser currentUser = await userManager.FindByNameAsync(User.Identity.Name);
            IFormFile uploadedImage = Request.Form.Files["Image"];
            if (uploadedImage != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Account");
                string fileName = Guid.NewGuid().ToString() + uploadedImage.FileName;
                string fullPath = Path.Combine(path, fileName);
                uploadedImage.CopyTo(new FileStream(fullPath, FileMode.Create));

                currentUser.ImagePath = "~/Images/Account/" + fileName;
                await userManager.UpdateAsync(currentUser);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Policy = "LoginRestricted")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Account/OrderHistory")]
        [Authorize(Policy = "LoginRestricted")]
        public ActionResult OrderHistory()
        {
            List<OrderWithTotal> ordersWithTotals = new List<OrderWithTotal>();
            AppUser currentUser = userManager.FindByNameAsync(User.Identity.Name).Result;
            List<Order> allOrders = gameStoreDbContext.Orders.Where(o => o.UserId == currentUser.Id).Include(o => o.Lines).ThenInclude(v => v.VideoGame).ToList();
            allOrders.ForEach(o => {
                ordersWithTotals.Add(new OrderWithTotal
                    { Order = o, Total = o.Lines.Sum(e => e.VideoGame.Price * e.Quantity) });
            });

            return View(ordersWithTotals);
        }
    }
}
