using Core.Entities;

namespace Core
{
    public interface IMessageRepository
    {
        Task<Tweet> GetOneAsync(string userName, string id, CancellationToken cancellationToken);
        Task<bool> TweetExistsAsync(string id, CancellationToken cancellationToken);
        Task<bool> TweetExistsAsync(string userName, string id, CancellationToken cancellationToken);
        Task CreateAsync(Tweet tweet, CancellationToken cancellationToken);
        Task EditAsync(Tweet tweet, CancellationToken cancellationToken);
        Task DeleteAsync(string userName, string id, CancellationToken cancellationToken);
    }
}