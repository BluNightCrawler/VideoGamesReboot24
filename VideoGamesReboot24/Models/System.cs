using System.ComponentModel.DataAnnotations;

namespace VideoGamesReboot24.Models
{
    public class System
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public List<VideoGameFull> VideoGames { get; set; }
    }
}
