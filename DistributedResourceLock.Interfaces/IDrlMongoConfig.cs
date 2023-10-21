namespace DistributedResourceLock.Interfaces
{
    public interface IDrlMongoConfig
    {
        string DatabaseName { get; }
        string ConnectionString { get; }
        string Collection { get; }
    }
}
