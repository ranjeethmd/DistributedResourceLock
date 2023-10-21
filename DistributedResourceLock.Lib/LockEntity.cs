using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedResourceLock
{
    public class LockEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string ResourceId { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
