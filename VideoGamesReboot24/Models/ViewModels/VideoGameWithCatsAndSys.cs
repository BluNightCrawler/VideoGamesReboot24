using Microsoft.AspNetCore.Mvc.Rendering;

namespace VideoGamesReboot24.Models.ViewModels
{
    public class VideoGameWithCatsAndSys
    {
        public VideoGameFull VideoGame { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Systems { get; set; }

        public IEnumerable<int> CategoryIds { get; set; }
        public IEnumerable<int> SystemIds { get; set; }

        public List<SelectListItem> AgeRatings {  get; set; }
    }
}
