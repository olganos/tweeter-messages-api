using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Entities
{
    public class Reply
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string TweetId { get; set; }

        [BsonRequired]
        public string Text { get; set; }

        [BsonRequired]
        public string UserName { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        [BsonRequired]
        public DateTimeOffset Created { get; set; }
    }
}