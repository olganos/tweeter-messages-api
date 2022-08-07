using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class TweeterMessageService
    {
        private readonly IMongoCollection<Tweet> _tweeterMessageCollection;

        public TweeterMessageService(
            IOptions<TweeterMessageSettings> tweeterStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                tweeterStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                tweeterStoreDatabaseSettings.Value.DatabaseName);

            _tweeterMessageCollection = mongoDatabase.GetCollection<Tweet>("tweet");
        }

        public async Task<List<Tweet>> GetAsync() =>
            await _tweeterMessageCollection.Find(_ => true).ToListAsync();

        public async Task<Tweet?> GetAsync(string id) =>
            await _tweeterMessageCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Tweet newBook) =>
            await _tweeterMessageCollection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, Tweet updatedBook) =>
            await _tweeterMessageCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _tweeterMessageCollection.DeleteOneAsync(x => x.Id == id);
    }
}
