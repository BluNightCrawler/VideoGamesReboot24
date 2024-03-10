using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            if (!context.VideoGames.Any())
            {
                context.VideoGames.Add(
                    new VideoGameFull
                    {
                        Name = "Game1",
                        Description = "Test",
                        Price = 19.99,
                        Systems = context.Systems.Where(s => s.Name == "Game Boy" || s.Name == "Xbox 360").ToList()
                    }
                );
            }

            if (!context.Categories.Any())
            {
                string json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data\\categories.json"));
                JArray categories = JArray.Parse(json);
                foreach (JObject category in categories)
                {
                    Category newCat = new Category();
                    newCat.Name = (string)category["CategoryName"];
                    context.Add(newCat);
                }
            }
            if (!context.Systems.Any())
            {
                string json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Data\\systems.json"));
                JArray systems = JArray.Parse(json);
                foreach (JObject system in systems)
                {
                    System newSys = new System();
                    newSys.Name = (string)system["SystemName"];
                    context.Add(newSys);
                }
            }
            context.SaveChanges();
        }
    }
}