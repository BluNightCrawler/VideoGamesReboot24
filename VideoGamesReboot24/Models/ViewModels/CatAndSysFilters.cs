using Microsoft.AspNetCore.Mvc.Rendering;

namespace VideoGamesReboot24.Models.ViewModels
{
    public class CatAndSysFilters
    {
        public SelectList Categories { get; set; }
        public SelectList Systems { get; set; }
    }
}
