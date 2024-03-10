using Microsoft.EntityFrameworkCore;

namespace VideoGamesReboot24.Models
{
    public class GameStoreDbContext : DbContext
    {
        public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options)
        : base(options) { }
        public DbSet<VideoGame> Products => Set<VideoGame>();

        public DbSet<VideoGameFull> VideoGames => Set<VideoGameFull>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<System> Systems => Set<System>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VideoGameFull>()
                .HasMany(e => e.Categories)
                .WithMany(e => e.VideoGames)
                .UsingEntity("VideoGameCategories");

            modelBuilder.Entity<VideoGameFull>()
                .HasMany(e => e.Systems)
                .WithMany(e => e.VideoGames)
                .UsingEntity("VideoGameSystems");
        }

        public DbSet<ApiAccessToken> ApiAccessTokens => Set<ApiAccessToken>();
    }
}
