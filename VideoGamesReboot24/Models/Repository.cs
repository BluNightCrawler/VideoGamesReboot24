namespace VideoGamesReboot24.Models
{
    public class Repository : IStoreRepository
    {
        private GameStoreDbContext context;
        public Repository(GameStoreDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<VideoGame> Products => context.Products;
        public void CreateProduct(VideoGame game)
        {
            context.Add(game);
            context.SaveChanges();
        }
        public void DeleteProduct(VideoGame game)
        {
            context.Remove(game);
            context.SaveChanges();
        }
        public void SaveProduct(VideoGame game)
        {
            context.SaveChanges();
        }

    }
}
