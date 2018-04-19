using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Configuration;
using System;

namespace AwsParameterStore.Microsoft.Extensions.Configuration
{
    public static class AwsParameterStoreConfigurationExtensions
    {
        public static IConfigurationBuilder AddAwsParameterStore(
            this IConfigurationBuilder configurationBuilder,
            string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return AddAwsParameterStore(
                configurationBuilder,
                path,
                new DefaultAwsParameterManager(),
                new AmazonSimpleSystemsManagementClient());
        }
        public static IConfigurationBuilder AddAwsParameterStore(
            this IConfigurationBuilder configurationBuilder,
            string path,
            IAwsParameterManager parameterManager)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (parameterManager == null)
            {
                throw new ArgumentNullException(nameof(parameterManager));
            }
            return AddAwsParameterStore(
                configurationBuilder,
                path,
                parameterManager,
                new AmazonSimpleSystemsManagementClient());
        }

        public static IConfigurationBuilder AddAwsParameterStore(
            this IConfigurationBuilder configurationBuilder,
            string path,
            IAwsParameterManager parameterManager, IAmazonSimpleSystemsManagement amazonSimpleSystemsManagementClient)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (parameterManager == null)
            {
                throw new ArgumentNullException(nameof(parameterManager));
            }
            if (amazonSimpleSystemsManagementClient == null)
            {
                throw new ArgumentNullException(nameof(amazonSimpleSystemsManagementClient));
            }

            configurationBuilder.Add(new AwsParameterStoreConfigurationSource
            {
                Path = path,
                Client = amazonSimpleSystemsManagementClient,
                ParameterManager = parameterManager
            });
            return configurationBuilder;
        }
    }
}
