namespace VideoGamesReboot24.Models
{
    public interface IStoreRepository
    {
        IQueryable<VideoGame> Products { get; }
        void SaveProduct(VideoGame game);
        void CreateProduct(VideoGame game);
        void DeleteProduct(VideoGame game);
    }
}
