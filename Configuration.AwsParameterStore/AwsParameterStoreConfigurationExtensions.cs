using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Configuration;
using System;

namespace Configuration.AwsParameterStore
{
    public static class AwsParameterStoreConfigurationExtensions
    {
        public static IConfigurationBuilder AddAwsParameterStore(
            this IConfigurationBuilder configurationBuilder,
            string path,
            IAwsParameterManager parameterManager)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var client = new AmazonSimpleSystemsManagementClient();
            configurationBuilder.Add(new AwsParameterStoreConfigurationSource
            {
                Path = path,
                Client = client,
                ParameterManager = parameterManager
            });
            return configurationBuilder;
        }
    }
}
