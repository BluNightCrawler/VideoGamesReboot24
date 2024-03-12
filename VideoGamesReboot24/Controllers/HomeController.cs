using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGamesReboot24.Models;
using VideoGamesReboot24.Models.ViewModels;
using System.Data.SqlClient;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using VideoGamesReboot24.Infrastructure;
using Microsoft.AspNetCore.Http.Extensions;

namespace VideoGamesReboot24.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<AppUser> userManager;
        private IStoreRepository repository;
        private GameStoreDbContext gameStoreDbContext;
        //static List<SeedData> video ;
        public int PageSize = 8;

        public HomeController(IStoreRepository repo, UserManager<AppUser> userManager, GameStoreDbContext context)
        {
            repository = repo;
            this.userManager = userManager;
            this.gameStoreDbContext = context;
        }
        public ViewResult Index()
        {
            return View();
        }

        public ViewResult Catalog(string? category, int productPage = 1)
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
        [Route("Products/Catalog/Import")]
        [Authorize(Policy = "AdminRestricted")]
        public ViewResult Import()
        {
            return View("CatalogImport", new List<VideoGameFull>());
        }

        [HttpGet]
        [Route("AboutUs")]
        public ViewResult AboutUs()
        {
            return View("AboutUs");
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
            return RedirectToAction("Catalog");

        }

        [HttpGet]
        [Route("Products/Details/{id?}")]
        public ViewResult Details(long? id)
        {
            if (id == null) return (ViewResult)Error();

            VideoGame? game = repository.Products.FirstOrDefault(p => p.ProductID == id);

            if (game == null) return (ViewResult)Error();

            return View("VideoGamesDetails",game);
        }


        [HttpGet]
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

            return RedirectToAction("Catalog");
        }

        [HttpGet]
        [Route("Products/Delete/{id?}")]
        [Authorize(Policy = "AdminRestricted")]
        public ActionResult Delete(long? id)
        {
            if (id == null) return (ViewResult)Error();

            VideoGame? game = repository.Products.FirstOrDefault(p => p.ProductID == id);

            if (game == null) return (ViewResult)Error();

            repository.DeleteProduct(game);

            return RedirectToAction("Catalog");
        }

        [HttpPost]
        [Authorize(Policy = "AdminRestricted")]
        public ActionResult Search()
        {
            List<VideoGameFull> searchList = new List<VideoGameFull>();
            ApiHelper apiHelper = new ApiHelper(gameStoreDbContext, "IGDB");
            if ((string)Request.Form["SearchString"] == "")
            {
                ModelState.AddModelError(string.Empty, "Please input a search term.");
                return View("CatalogImport", searchList);
            }

            searchList = apiHelper.SearchVideoGame((string)Request.Form["SearchString"]);
            
            return View("CatalogImport", searchList);
        }

        [HttpGet]
        [Authorize(Policy = "AdminRestricted")]
        [Route("Home/AddToCatalog/{name}")]
        public ActionResult AddToCatalog(string name)
        {
            ApiHelper apiHelper = new ApiHelper(gameStoreDbContext, "IGDB");
            if (gameStoreDbContext.VideoGames.Where(v => v.Name == name).Any())
            {
                ModelState.AddModelError(string.Empty, "Game Already Exists in DB");
                return View("CatalogImport", new List<VideoGameFull>());
            }
            VideoGameFull? game = apiHelper.GetVideoGame(name);
            if (game is not null)
            {
                gameStoreDbContext.VideoGames.Add(game);
                gameStoreDbContext.SaveChanges();
            }
            return View("CatalogImport", new List<VideoGameFull>());
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