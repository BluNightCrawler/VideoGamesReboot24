namespace VideoGamesReboot24.Models
{
    public interface IStoreRepository
    {
        IQueryable<VideoGameFull> Products { get; }
        void SaveProduct(VideoGameFull game);
        void CreateProduct(VideoGameFull game);
        void DeleteProduct(VideoGameFull game);
    }
}
