using DistributedResourceLock.Lib;

namespace DistributedResourceLock.Test
{
    public class DistributedLockManagerTest
    {
        public static DistributedLockManager Arrange()
        {
            return new DistributedLockManager(new DrlMongoDbConfig());
        }

        [Fact]
        public async Task CanTryAcquireLockAsyncAsync()
        {
            var lockManager = Arrange();

            var error = await Record.ExceptionAsync(async () =>
            {
                using (await lockManager.TryAcquireLockAsync("Dummy", 3))
                {
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            });

            Assert.Null(error);
        }

        [Fact]
        public async Task CanFailTryAcquireLockAsyncAsync()
        {
            var lockManager = Arrange();

            var lockObj = await lockManager.TryAcquireLockAsync("Dummy1", 1);


            var nullLock = await lockManager.TryAcquireLockAsync("Dummy1", 1, 1);

           

            lockObj.Dispose();

            Assert.Null(nullLock);
        }
    }
}