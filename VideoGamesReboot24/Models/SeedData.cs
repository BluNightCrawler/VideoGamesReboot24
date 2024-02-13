using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

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
                //string json = File.ReadAllText("test.json");
                //VideoGame[]? videoGames = JsonConvert.DeserializeObject<VideoGame[]>(json);
                context.Products.AddRange(
                new VideoGame
                {
                    ProductName = "Kayak",
                    Description = "A boat for one person",
                    Category = "Watersports",
                    Price = 275,
                    System = "NA"
                },
                new VideoGame
                {
                    ProductName = "Lifejacket",
                    Description = "Protective and fashionable",
                    Category = "Watersports",
                    Price = 48.95,
                    System = "NA"
                },
                new VideoGame
                {
                    ProductName = "Soccer Ball",
                    Description = "FIFA-approved size and weight",
                    Category = "Soccer",
                    Price = 19.50,
                    System = "NA"
                },
                new VideoGame
                {
                    ProductName = "Corner Flags",
                    Description = "Give your playing field a professional touch",
                    Category = "Soccer",
                    Price = 34.95,
                    System = "NA"
                },
                new VideoGame
                {
                    ProductName = "Stadium",
                    Description = "Flat-packed 35,000-seat stadium",
                    Category = "Soccer",
                    Price = 79500,
                    System = "NA"
                },
                new VideoGame
                {
                    ProductName = "Thinking Cap",
                    Description = "Improve brain efficiency by 75%",
                    Category = "Chess",
                    Price = 16,
                    System = "NA"
                },
                new VideoGame
                {
                    ProductName = "Unsteady Chair",
                    Description = "Secretly give your opponent a disadvantage",
                    Category = "Chess",
                    Price = 29.95,
                    System = "NA"
                },
                new VideoGame
                {
                    ProductName = "Human Chess Board",
                    Description = "A fun game for the family",
                    Category = "Chess",
                    Price = 75,
                    System = "NA"
                },
                new VideoGame
                {
                    ProductName = "Bling-Bling King",
                    Description = "Gold-plated, diamond-studded King",
                    Category = "Chess",
                    Price = 1200,
                    System = "NA"
                }
                );
            }
            context.SaveChanges();
        }
    }
}