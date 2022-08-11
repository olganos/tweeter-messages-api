namespace DataLayer
{
    public interface IMessageRepository
    {
        Task<List<Tweet>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<Tweet>> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task CreateAsync(Tweet tweet, CancellationToken cancellationToken);
        Task EditAsync(Tweet tweet, CancellationToken cancellationToken);
        Task DeleteAsync(string userName, string id, CancellationToken cancellationToken);
    }
}