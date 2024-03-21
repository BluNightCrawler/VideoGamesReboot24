using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VideoGamesReboot24.Models;
using VideoGamesReboot24.Models.ViewModels;

namespace VideoGamesReboot24.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private IStoreRepository repository;
        public NavigationMenuViewComponent(IStoreRepository repo)
        {
            repository = repo;
        }
        public IViewComponentResult Invoke()
        {
            var selectedCat = RouteData?.Values["category"];
            ViewBag.SelectedCategory = selectedCat;

            var selectedSys = Request.Query["system"].ToString();
            ViewBag.SelectedSystem = selectedSys;

            var productCats = repository.Products
                .Include(x => x.Categories)
                .SelectMany(x => x.Categories)
                .Select(x => x.Name)
                .Distinct()
                .ToList();
            productCats.Add("");

            var productSys = repository.Products
                .Include(x => x.Systems)
                .SelectMany(x => x.Systems)
                .Select(x => x.Name)
                .Distinct()
                .ToList();
            productSys.Add("");

            SelectList catsWSelect = new SelectList(productCats.OrderBy(p => p));
            if (selectedCat != null)
            {
                catsWSelect.Where(c => c.Text == (string)selectedCat).First().Selected = true;
            }

            SelectList sysWSelect = new SelectList(productSys.OrderBy(p => p));
            if (selectedSys != null)
            {
                sysWSelect.Where(c => c.Text == (string)selectedSys).First().Selected = true;
            }
            return View(new CatAndSysFilters { Categories = catsWSelect, Systems = sysWSelect }); ;
        }
    }
}
