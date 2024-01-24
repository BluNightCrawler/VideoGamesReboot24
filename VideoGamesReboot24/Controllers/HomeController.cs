using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGamesReboot24.Models;
using VideoGamesReboot24.Models.ViewModels;
using System.Data.SqlClient;
using System;

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

        public ViewResult Index(int productPage = 1)
            => View(new VideoGameListViewModel {
                VideoGames = repository.Products
                    .OrderBy(p => p.ProductID)
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Products.Count()
                }
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
        /*public ActionResult Details(int? id)
        {
           
        }*/

        public ActionResult Edit(int id)
        {
            //VideoGame game = VideoGame.
            return View();
        }

        [HttpPost]
        public ActionResult Edit(VideoGame vdg)
        {
            var Name = vdg.ProductName;
            var price = vdg.Price;
            var des = vdg.Description;
            var cat = vdg.Category;
            var plat = vdg.System;

            return RedirectToAction("Index");
        }
        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}