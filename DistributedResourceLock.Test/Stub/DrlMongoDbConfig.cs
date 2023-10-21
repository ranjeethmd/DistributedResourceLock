using DistributedResourceLock.Interfaces;


namespace DistributedResourceLock
{
    public class DrlMongoDbConfig: IDrlMongoConfig
    {
        public string DatabaseName => "*****";
        public string Collection => "*****";
           
        public string ConnectionString => "*******";
    }
}
