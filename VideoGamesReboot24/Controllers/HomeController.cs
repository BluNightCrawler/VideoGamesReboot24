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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VideoGamesReboot24.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<AppUser> userManager;
        private IStoreRepository repository;
        private GameStoreDbContext gameStoreDbContext;
        //static List<SeedData> video ;
        public int PageSize = 8;

        public HomeController(IStoreRepository repo, UserManager<AppUser> userManager,
            GameStoreDbContext context)
        {
            repository = repo;
            this.userManager = userManager;
            this.gameStoreDbContext = context;
        }
        
        public ActionResult Index()
        {
            List<string> imageList = gameStoreDbContext.VideoGames.Select(p => p.ImagePath).Take(20).ToList();

            return View(imageList);
        }

        public ViewResult Catalog(string? category, string? system, int productPage = 1)
        {
            var products = repository.Products
                    .Include(p => p.Categories)
                    .Include(p => p.Systems)
                    .Where(p => category == null || p.Categories.Any(c => c.Name == category))
                    .Where(p => system == null || p.Systems.Any(s => s.Name == system))
                    .OrderBy(p => p.Id);
            return View(new VideoGameListViewModel
            {
                VideoGames = products
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = products.Count()
                },
                CurrentCategory = category,
                CurrentSystem = system
            });
        }

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

            //repository.CreateProduct(game);

            TempData["NewGame"] = true;
            return RedirectToAction("Catalog");

        }

        [HttpGet]
        [Route("Products/Details/{id?}")]
        public ViewResult Details(long? id)
        {
            if (id == null) return (ViewResult)Error();

            VideoGameFull? game = repository.Products.Include(p => p.Categories).Include(p => p.Systems).FirstOrDefault(p => p.Id == id);

            if (game == null) return (ViewResult)Error();

            return View("VideoGamesDetails",game);
        }


        [HttpGet]
        [Route("Products/Edit/{id?}")]
        [Authorize(Policy = "AdminRestricted")]
        public ActionResult Edit(long? id)
        {
            if (id == null) return (ViewResult)Error();

            VideoGameFull? game = repository.Products.Include(x=> x.Categories).Include(x=>x.Systems).FirstOrDefault(p => p.Id == id);

            if (game == null) return (ViewResult)Error();

            List<SelectListItem> cats = new List<SelectListItem>();
            List<SelectListItem> syss = new List<SelectListItem>();
            List<SelectListItem> ageratings = new List<SelectListItem>();
            foreach (var cat in gameStoreDbContext.Categories.ToList())
            {
                cats.Add(new SelectListItem
                {
                    Text = cat.Name,
                    Value = cat.Id.ToString(),
                    Selected = game.Categories.Contains(cat)
                });
            }
            foreach (var sys in gameStoreDbContext.Systems.ToList())
            {
                syss.Add(new SelectListItem
                {
                    Text=sys.Name,
                    Value = sys.Id.ToString(),
                    Selected = game.Systems.Contains(sys)
                });
            }
            var agerating = Enum.GetNames(typeof(Ratings));
            for(int i = 0; i < agerating.Length; i++)
            {
                ageratings.Add(new SelectListItem
                {
                    Text = agerating[i],
                    Value = agerating[i],
                    Selected = game.AgeRating == agerating[i]
                });
            }

            VideoGameWithCatsAndSys viewModel = new VideoGameWithCatsAndSys
            {
                VideoGame = game,
                Categories = cats,
                Systems = syss,
                AgeRatings = ageratings
            };

            return View("VideoGamesEdit", viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "AdminRestricted")]
        public ActionResult Edit(VideoGameWithCatsAndSys gameModel)
        {
            ModelState.Remove("VideoGame.Systems");
            ModelState.Remove("VideoGame.Categories");
            ModelState.Remove("Categories");
            ModelState.Remove("Systems");
            ModelState.Remove("AgeRatings");
            if (!ModelState.IsValid) { return View("VideoGamesEdit", gameModel); }
            
            VideoGameFull game = gameModel.VideoGame;

            VideoGameFull? gameEdit = repository.Products.Include(x=>x.Categories).Include(x=>x.Systems).FirstOrDefault(p => p.Id == game.Id);

            if (gameEdit == null) return (ViewResult)Error();

            gameEdit.Name = game.Name;
            gameEdit.Description = game.Description;
            gameEdit.Price = game.Price;
            gameEdit.AgeRating = game.AgeRating;
            gameEdit.LongImagePath = game.LongImagePath;
            gameEdit.Rating = game.Rating;
            gameEdit.RatingCount = game.RatingCount;
            gameEdit.ReleaseDate = game.ReleaseDate;

            gameEdit.Categories = gameStoreDbContext.Categories.Where(s => gameModel.CategoryIds.Contains(s.Id)).ToList();
            gameEdit.Systems = gameStoreDbContext.Systems.Where(s => gameModel.SystemIds.Contains(s.Id)).ToList();

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

            VideoGameFull? game = repository.Products.FirstOrDefault(p => p.Id == id);

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

        [HttpPost]
        public ActionResult FilterSearch()
        {
            if ((string)Request.Form["SearchString"] == "")
            {
                return View("Catalog");
            }

            return View("Catalog", new VideoGameListViewModel
            {
                VideoGames = repository.Products
                    .Include(p => p.Categories)
                    .Include(p => p.Systems)
                    .Where(p => p.Name.Contains((string)Request.Form["SearchString"]))
                    .OrderBy(p => p.Id)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Products.Where(p => p.Name.Contains((string)Request.Form["SearchString"])).Count()
                },
                CurrentCategory = null
            });
        }

        [HttpPost]
        public ActionResult Filter(IFormCollection form)
        {
            return RedirectToAction("Catalog", new { category = (string)form["category-select-filter"], system = (string)form["systems-select-filter"], productPage = 1});
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