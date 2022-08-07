namespace DataLayer
{
    public interface IMessageRepository
    {
        Task<List<Tweet>> GetAllAsync();
        Task CreateAsync(Tweet tweet);
    }
}