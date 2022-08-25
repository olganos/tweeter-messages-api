using Core;
using Core.Entities;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Tweet> _tweetsCollection;
        private readonly IMongoCollection<Reply> _repliesCollection;

        public MessageRepository(string connectionString, string databaseName,
            string tweetCollectionName, string replyCollectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _tweetsCollection = database.GetCollection<Tweet>(tweetCollectionName);
            _repliesCollection = database.GetCollection<Reply>(replyCollectionName);
        }

        public async Task<Tweet> GetOneAsync(string userName, string id, CancellationToken cancellationToken)
        {
            return await _tweetsCollection
                .Find(p => p.Id == id && p.UserName == userName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> TweetExistsAsync(string id, CancellationToken cancellationToken) =>
            await _tweetsCollection
                .Find(p => p.Id == id)
                .AnyAsync(cancellationToken);

        public async Task<bool> TweetExistsAsync(string userName, string id, CancellationToken cancellationToken) =>
            await _tweetsCollection
                .Find(p => p.Id == id && p.UserName == userName)
                .AnyAsync(cancellationToken);

        public async Task CreateAsync(Tweet tweet, CancellationToken cancellationToken) =>
            await _tweetsCollection.InsertOneAsync(
                tweet,
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

        public async Task EditAsync(Tweet tweet, CancellationToken cancellationToken) =>
            await _tweetsCollection
                .ReplaceOneAsync(
                    filter: g => g.Id == tweet.Id,
                    replacement: tweet,
                    cancellationToken: cancellationToken);

        public async Task DeleteAsync(string userName, string id, CancellationToken cancellationToken)
        {
            await _repliesCollection.DeleteManyAsync(p => p.TweetId == id, cancellationToken);
            await _tweetsCollection.DeleteOneAsync(p => p.Id == id && p.UserName == userName, cancellationToken);
        }
    }
}
