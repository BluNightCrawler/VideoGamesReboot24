using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGamesReboot24.Models
{
    public class VideoGameFull
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public double Price { get; set; }

        public List<Category> Categories { get; set; }

        public List<System> Systems { get; set; }

        public string? AgeRating { get; set; }

        public string? ImagePath { get; set; }

        public string? LongImagePath { get; set; }

        public double? Rating { get; set; }

        public int? RatingCount { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public bool Active { get; set; }
    }
}
