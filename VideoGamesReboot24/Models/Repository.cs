namespace VideoGamesReboot24.Models
{
    public class Repository : IStoreRepository
    {
        private GameStoreDbContext context;
        public Repository(GameStoreDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<VideoGameFull> Products => context.VideoGames;
        public void CreateProduct(VideoGameFull game)
        {
            context.Add(game);
            context.SaveChanges();
        }
        public void DeleteProduct(VideoGameFull game)
        {
            context.Remove(game);
            context.SaveChanges();
        }
        public void SaveProduct(VideoGameFull game)
        {
            context.SaveChanges();
        }

    }
}
