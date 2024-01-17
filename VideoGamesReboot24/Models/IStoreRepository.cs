namespace VideoGamesReboot24.Models
{
    public interface IStoreRepository
    {
        IQueryable<VideoGame> Products { get; }
    }
}
