using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGamesReboot24.Models;

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
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(repository.Products
                .Include(x => x.Categories)
                .SelectMany(x => x.Categories)
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x));
        }
    }
}
