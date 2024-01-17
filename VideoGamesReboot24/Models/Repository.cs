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

    }
}
