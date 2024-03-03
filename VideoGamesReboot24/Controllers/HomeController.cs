using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGamesReboot24.Models;
using VideoGamesReboot24.Models.ViewModels;
using System.Data.SqlClient;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace VideoGamesReboot24.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<AppUser> userManager;
        private IStoreRepository repository;
        //static List<SeedData> video ;
        public int PageSize = 8;

        public HomeController(IStoreRepository repo, UserManager<AppUser> userManager)
        {
            repository = repo;
            this.userManager = userManager;
        }

        public ViewResult Index(string? category, int productPage = 1)
            => View(new VideoGameListViewModel {
                VideoGames = repository.Products
                    .Where(p => category == null || p.Category == category)
                    .OrderBy(p => p.ProductID)
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = (category==null? repository.Products.Count() :
                        repository.Products.Where(e => e.Category == category).Count())
                },
                CurrentCategory = category
            });



        [HttpGet]
        [Route("Home/AboutUs")]
        [Route("Products/AboutUs")]
        public ViewResult AboutUs()
        {
            return View("AboutUs");
        }
        [HttpGet]
        [Route("Home/Catalog")]
        [Route("Products/Catalog")]
        public ViewResult Catalog()
        {
            return View("Catalog");
        }

        [HttpGet]
        [Route("Products/Create")]
        [Authorize(Policy = "AdminRestricted")]
        public ViewResult Create()
        {
            return View("VideoGameForm");
        }

        [HttpPost]
        [Route("Products/Create")]
        [Authorize(Policy = "AdminRestricted")]
        public ActionResult VideoGameForm(VideoGame game)
        {
            if (!ModelState.IsValid) { return View(game); }

            IFormFile uploadedImage = Request.Form.Files["Image"];
            if (uploadedImage != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Game");
                string fileName = Guid.NewGuid().ToString() + uploadedImage.FileName;
                string fullPath = Path.Combine(path, fileName);
                uploadedImage.CopyTo(new FileStream(fullPath, FileMode.Create));
                game.ImagePath = "~/Images/Game/" + fileName;
            }
            else
            {
                game.ImagePath = "";
            }

            repository.CreateProduct(game);

            TempData["NewGame"] = true;
            return RedirectToAction("Index");

        }

        [HttpGet]
        [Route("Home/Details/{id?}")]
        [Route("Products/Details/{id?}")]
        public ViewResult Details(long? id)
        {
            if (id == null) return (ViewResult)Error();

            VideoGame? game = repository.Products.FirstOrDefault(p => p.ProductID == id);

            if (game == null) return (ViewResult)Error();

            return View("VideoGamesDetails",game);
        }


        [HttpGet]
        [Route("Home/Edit/{id?}")]
        [Route("Products/Edit/{id?}")]
        [Authorize(Policy = "AdminRestricted")]
        public ActionResult Edit(long? id)
        {
            if (id == null) return (ViewResult)Error();

            VideoGame? game = repository.Products.FirstOrDefault(p => p.ProductID == id);

            if (game == null) return (ViewResult)Error();

            return View("VideoGamesEdit", game);
        }

        [HttpPost]
        [Authorize(Policy = "AdminRestricted")]
        public ActionResult Edit(VideoGame game)
        {
            if (!ModelState.IsValid) { return View("VideoGamesEdit", game); }
            

            VideoGame? gameEdit = repository.Products.FirstOrDefault(p => p.ProductID == game.ProductID);

            if (gameEdit == null) return (ViewResult)Error();

            gameEdit.ProductName = game.ProductName;
            gameEdit.Price = game.Price;
            gameEdit.Description = game.Description;
            gameEdit.Category = game.Category;
            gameEdit.System = game.System;

            IFormFile uploadedImage = Request.Form.Files["Image"];
            if (uploadedImage != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Game");
                string fileName = Guid.NewGuid().ToString() + uploadedImage.FileName;
                string fullPath = Path.Combine(path, fileName);
                uploadedImage.CopyTo(new FileStream(fullPath, FileMode.Create));
                gameEdit.ImagePath = "~/Images/Game/" + fileName;
            }
            else
            {
                gameEdit.ImagePath = game.ImagePath == "$" ? "" : game.ImagePath;
            }

            repository.SaveProduct(game);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Home/Delete/{id?}")]
        [Route("Products/Delete/{id?}")]
        [Authorize(Policy = "AdminRestricted")]
        public ActionResult Delete(long? id)
        {
            if (id == null) return (ViewResult)Error();

            VideoGame? game = repository.Products.FirstOrDefault(p => p.ProductID == id);

            if (game == null) return (ViewResult)Error();

            repository.DeleteProduct(game);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //helper
        public string GetUserImage()
        {
            AppUser user = userManager.FindByNameAsync(User.Identity.Name).GetAwaiter().GetResult();
            if (user.ImagePath == "")
            {
                return "~/Images/placeholder.jpg";
            }
            return user.ImagePath;
        }
       
    }
}