using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DistributedResourceLock.Lib
{
    public class LockEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string ResourceId { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
