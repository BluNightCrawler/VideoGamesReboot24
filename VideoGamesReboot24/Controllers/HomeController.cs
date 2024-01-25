using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGamesReboot24.Models;
using VideoGamesReboot24.Models.ViewModels;
using System.Data.SqlClient;
using System;
using Microsoft.EntityFrameworkCore;

namespace VideoGamesReboot24.Controllers
{
    public class HomeController : Controller
    {
        
        private IStoreRepository repository;
        //static List<SeedData> video ;
        public int PageSize = 8;

        public HomeController(IStoreRepository repo)
        {
            repository = repo;
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
        public ViewResult VideoGameForm()
        {
            return View();
        }

        [HttpPost]
        public ViewResult VideoGameForm(VideoGame product)
        {
            if (ModelState.IsValid)
            {
                /*Repository.AddResponse(product);*/
                return View("VideoGameThanks", product);
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        [Route("Home/Details/{id?}")]
        [Route("Products/Details/{id?}")]
        public ViewResult Details(long? id)
        {
            if (id == null) return (ViewResult)Error();

            VideoGame? game = repository.Products
                .FirstOrDefault(p => p.ProductID == id);

            if (game == null) return (ViewResult)Error();

            return View("VideoGamesDetails",game);
        }

        [HttpGet]
        [Route("Home/Edit/{id?}")]
        [Route("Products/Edit/{id?}")]
        public ActionResult Edit(long? id)
        {
            if (id == null) return (ViewResult)Error();

            VideoGame? game = repository.Products.FirstOrDefault(p => p.ProductID == id);

            if (game == null) return (ViewResult)Error();

            return View("VideoGamesEdit", game);
        }

        [HttpPost]
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

            repository.SaveProduct(game);

            return RedirectToAction("Index");
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}