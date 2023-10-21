using DistributedResourceLock.Interfaces;


namespace DistributedResourceLock
{
    public class DrlMongoDbConfig : IDrlMongoConfig
    {
        public string DatabaseName => "IFS";
        public string Collection => "DRL";

        public string ConnectionString => "mongodb://IFS_rw:sH48tA4Up2eQg4c@d1fm1mon232.amr.corp.intel.com:8651,d2fm1mon232.amr.corp.intel.com:8651,dr1or1mon232fm1.amr.corp.intel.com:8651/IFS?ssl=true&replicaSet=mongo8651&tlsAllowInvalidCertificates=true";
    }
}
