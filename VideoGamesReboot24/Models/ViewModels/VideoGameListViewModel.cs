namespace VideoGamesReboot24.Models.ViewModels
{
    public class VideoGameListViewModel
    {
        public IEnumerable<VideoGameFull> VideoGames { get; set; }
            = Enumerable.Empty<VideoGameFull>();
        public PagingInfo PagingInfo { get; set; } = new();
        public string? CurrentCategory { get; set; }
        public string? CurrentSystem { get; set; }
    }
}
