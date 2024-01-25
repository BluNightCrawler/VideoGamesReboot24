namespace VideoGamesReboot24.Models.ViewModels
{
    public class VideoGameListViewModel
    {
        public IEnumerable<VideoGame> VideoGames { get; set; }
            = Enumerable.Empty<VideoGame>();
        public PagingInfo PagingInfo { get; set; } = new();
        public string? CurrentCategory { get; set; }
    }
}
