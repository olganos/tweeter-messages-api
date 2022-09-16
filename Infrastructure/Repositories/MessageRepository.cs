using Core;
using Core.Entities;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Tweet> _tweetsCollection;
        private readonly IMongoCollection<Reply> _repliesCollection;
        private readonly IMongoCollection<Like> _likesCollection;

        public MessageRepository(string connectionString, string databaseName,
            string tweetCollectionName, string replyCollectionName, string likeCollectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _tweetsCollection = database.GetCollection<Tweet>(tweetCollectionName);
            _repliesCollection = database.GetCollection<Reply>(replyCollectionName);
            _likesCollection = database.GetCollection<Like>(likeCollectionName);
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
            await _likesCollection.DeleteManyAsync(p => p.TweetId == id && p.UserName == userName, cancellationToken);
        }

        public async Task LikeAsync(Like like, CancellationToken cancellationToken) =>
           await _likesCollection.InsertOneAsync(
               like,
               new InsertOneOptions { BypassDocumentValidation = false },
               cancellationToken);
    }
}
