namespace VideoGamesReboot24.Models.ViewModels
{
    public class VideoGameWithCatsAndSys
    {
        public VideoGameFull VideoGame {  get; set; }
        public List<Category> Categories { get; set; }
        public List<System> Systems { get; set; }
    }
}
