using DistributedResourceLock.Lib;

namespace DistributedResourceLock
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var lockManager = new DistributedLockManager(new DrlMongoDbConfig());

            Console.WriteLine(DateTime.Now);

            try
            {
                using (await lockManager.TryAcquireLockAsync("SomeThing",3))
                {
                    Console.WriteLine("Acquired lock");
                   await Task.Delay(TimeSpan.FromMinutes(5));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Complete");

            Console.ReadLine();
        }
    }
}