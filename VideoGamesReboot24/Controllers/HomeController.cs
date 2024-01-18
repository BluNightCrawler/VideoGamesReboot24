using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGamesReboot24.Models;

namespace VideoGamesReboot24.Controllers
{
    public class HomeController : Controller
    {
        private IStoreRepository repository;

        public HomeController(IStoreRepository repo)
        {
            repository = repo;
        }

        public IActionResult Index() => View(repository.Products);

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
                //Repository.AddResponse(product);
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

        public ActionResult Edit(int Id)
        {
            var vdg = VideoGame.Where(V => V.ProductID == Id).FirstOrDefault();

            return View(vdg);
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