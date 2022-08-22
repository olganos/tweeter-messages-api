using Core;
using Core.Entities;
using MongoDB.Driver;

namespace DataLayer
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Tweet> _tweetsCollection;

        public MessageRepository()
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            var databaseName = Environment.GetEnvironmentVariable("DB_NAME");

            var client = new MongoClient(connectionString
                ?? "mongodb://localhost:8007/?readPreference=primary&ssl=false");

            var database = client.GetDatabase(databaseName
                ?? "tweeter-messages");

            _tweetsCollection = database.GetCollection<Tweet>("tweet");
        }

        public async Task<List<Tweet>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _tweetsCollection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task<List<Tweet>> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            FilterDefinition<Tweet> filter = Builders<Tweet>.Filter.Eq(p => p.UserName, username);

            return await _tweetsCollection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<Tweet> GetOneAsync(string userName, string id, CancellationToken cancellationToken)
        {
            return await _tweetsCollection
                .Find(p => p.Id == id && p.UserName == userName)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<Tweet> GetOneAsync(string id, CancellationToken cancellationToken)
        {
            return await _tweetsCollection
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

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

        public async Task DeleteAsync(string userName, string id, CancellationToken cancellationToken) =>
            await _tweetsCollection.DeleteOneAsync(p => p.Id == id && p.UserName == userName, cancellationToken);
    }
}
