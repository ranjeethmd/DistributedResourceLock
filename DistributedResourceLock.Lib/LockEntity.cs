using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DistributedResourceLock.Lib
{
    public class LockEntity
    {
        [BsonId]
        public string Id { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
