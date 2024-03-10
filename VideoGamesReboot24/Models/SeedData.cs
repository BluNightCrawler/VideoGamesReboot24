using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace VideoGamesReboot24.Models
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            GameStoreDbContext context = app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<GameStoreDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            if (!context.Products.Any())
            {
                string json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data\\products.json"));
                List<VideoGame> videoGames = JsonConvert.DeserializeObject<List<VideoGame>>(json);
                context.Products.AddRange(videoGames);
            }
            context.SaveChanges();
        }
    }
}