using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using NuGet.Protocol.Core.Types;
using VideoGamesReboot24.Infrastructure;
using VideoGamesReboot24.Models;

namespace VideoGamesReboot24.Controllers
{

    [Authorize(Policy = "LoginRestricted")]
    public class OrderController : Controller
    {
        private GameStoreDbContext gameStoreDbContext;
        private UserManager<AppUser> userManager;

        public OrderController(GameStoreDbContext context, UserManager<AppUser> userManager)
        {
            this.gameStoreDbContext = context;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Cart()
        {
            return View(HttpContext.Session.GetJson<Cart>("cart") ?? new Cart());
        }

        [HttpGet]
        public IActionResult AddToCart(int id)
        {
            VideoGameFull? game = gameStoreDbContext.VideoGames
                .FirstOrDefault(p => p.Id == id);
            if (game != null)
            {
                Cart cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                cart.AddItem(game, 1);
                HttpContext.Session.SetJson("cart", cart);
            }
            return RedirectToAction("Catalog", "Home");
        }

        [HttpGet]
        public IActionResult RemoveFromCart(int id)
        {
            VideoGameFull? game = gameStoreDbContext.VideoGames
                .FirstOrDefault(p => p.Id == id);
            if (game != null)
            {
                Cart cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                cart.RemoveLine(game);
                HttpContext.Session.SetJson("cart", cart);
            }
            return RedirectToAction("Cart");
        }


        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            Cart cart = HttpContext.Session.GetJson<Cart>("cart")!;
            order.Lines = cart.Lines.ToArray();
            order.UserId = userManager.FindByNameAsync(User.Identity!.Name).Result.Id;
            ModelState.Remove("UserId");
            if (order.Lines.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty!");
                return View();
            }
            if (!ModelState.IsValid) return View();

            gameStoreDbContext.AttachRange(order.Lines.Select(l => l.VideoGame));
            Order returnOrder = new Order();
            if (order.OrderID == 0)
            {
                returnOrder = gameStoreDbContext.Orders.Add(order).Entity;
            }
            gameStoreDbContext.SaveChanges();
            cart.Clear();
            HttpContext.Session.SetJson("cart", cart);
            return View("Completed", returnOrder);
        }
    }
}
