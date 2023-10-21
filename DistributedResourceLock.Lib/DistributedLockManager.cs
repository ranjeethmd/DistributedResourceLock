using DistributedResourceLock.Interfaces;
using MongoDB.Driver;

namespace DistributedResourceLock.Lib
{
    public class DistributedLockManager
    {
        private readonly IMongoCollection<LockEntity> _collection;

        public DistributedLockManager(IDrlMongoConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            var db = client.GetDatabase(config.DatabaseName);

            _collection = db.GetCollection<LockEntity>(config.Collection);

            _collection.Indexes.CreateOne(new CreateIndexModel<LockEntity>(
                Builders<LockEntity>.IndexKeys.Ascending(l => l.ExpiresAt),
                new CreateIndexOptions
                {
                    ExpireAfter = TimeSpan.Zero
                }
            ));

            _collection.Indexes.CreateOne(new CreateIndexModel<LockEntity>(
                Builders<LockEntity>.IndexKeys.Ascending(l => l.ResourceId),
                new CreateIndexOptions
                {
                    Unique = true
                }
            ));
        }

        public async Task<IDisposable> TryAcquireLockAsync(string resourceId, int expirationInMins, int totalWaitInSec = 10)
        {
            var startTime = DateTime.UtcNow;

            while ((DateTime.UtcNow - startTime).TotalSeconds < totalWaitInSec)
            {
                try
                {
                    var lockEntity = await _collection.FindOneAndUpdateAsync<LockEntity>(
                        x => x.ResourceId == resourceId,
                        Builders<LockEntity>.Update
                            .SetOnInsert(x => x.ResourceId, resourceId)
                            .SetOnInsert(x => x.ExpiresAt, DateTime.UtcNow.AddMinutes(expirationInMins)),
                        new FindOneAndUpdateOptions<LockEntity>
                        {
                            IsUpsert = true,

                            ReturnDocument = ReturnDocument.Before
                        });

                    if (lockEntity == null)
                    {
                        return new _Lock(resourceId, _collection, expirationInMins);
                    }
                }
                catch (MongoCommandException ex)
                {
                    if (ex.Code == 11000)
                    {
                        // Two threads have tried to acquire a lock at the exact same moment on the same key, 
                        // which will cause a duplicate key exception in MongoDB.
                        // So this thread failed to acquire the lock.Ignore

                    }
                }

                await Task.Delay(250);
            }


            return null;

        }

        private class _Lock : IDisposable
        {
            private readonly IMongoCollection<LockEntity> _collection;
            private readonly CancellationTokenSource _source;

            public _Lock(string resourceId, IMongoCollection<LockEntity> collection, int windowLength)
            {
                ResourceId = resourceId;
                _collection = collection;
                _source = new CancellationTokenSource();
                CancellationToken cancellationToken = _source.Token;

                Task.Run(async () =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {

                        await _collection.UpdateOneAsync(x => x.ResourceId == resourceId,
                            Builders<LockEntity>.Update.Set(x => x.ExpiresAt,
                                DateTime.UtcNow + TimeSpan.FromMinutes(windowLength)));
                        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                    }

                }, cancellationToken);
            }

            public string ResourceId { get; }

            public void Dispose()
            {
                _source.Cancel();
                _collection.DeleteOne(x => x.ResourceId == ResourceId);
            }
        }
    }
}
