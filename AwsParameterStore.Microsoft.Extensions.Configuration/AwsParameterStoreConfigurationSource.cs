using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Configuration;

namespace AwsParameterStore.Microsoft.Extensions.Configuration
{
    internal class AwsParameterStoreConfigurationSource : IConfigurationSource
    {
        public IAmazonSimpleSystemsManagement Client { get; set; }
        public string Path { get; set; }
        public IAwsParameterManager ParameterManager { get; set; }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AwsParameterStoreConfigurationProvider(Client, Path, ParameterManager);
        }
    }
}
