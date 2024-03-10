using Microsoft.EntityFrameworkCore;

namespace VideoGamesReboot24.Models
{
    public class GameStoreDbContext : DbContext
    {
        public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options)
        : base(options) { }
        public DbSet<VideoGame> Products => Set<VideoGame>();
        public DbSet<ApiAccessToken> ApiAccessTokens => Set<ApiAccessToken>();
    }
}
