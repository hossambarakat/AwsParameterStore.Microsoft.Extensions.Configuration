using Microsoft.Extensions.Configuration;
using Xunit;

namespace AwsParameterStore.Microsoft.Extensions.Configuration.Tests
{
    public static class ConfigurationProviderExtensions
    {
        public static void ShouldHaveKeyWithValue(this IConfigurationProvider provider, string key, string expectedvalue)
        {
            string value;
            provider.TryGet(key, out value);
            Assert.Equal(expectedvalue, value);
        }
    }
}
