using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Tweet> _tweetsCollection;

        public MessageRepository(IMongoDatabase mongoDatabase)
        {
            _tweetsCollection = mongoDatabase.GetCollection<Tweet>("tweet");
        }

        public async Task<List<Tweet>> GetAllAsync()
        {
            return await _tweetsCollection.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(Tweet tweet)
        {
            await _tweetsCollection.InsertOneAsync(tweet); ;
        }
    }
}
