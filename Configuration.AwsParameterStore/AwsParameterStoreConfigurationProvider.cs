using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Configuration.AwsParameterStore
{
    internal class AwsParameterStoreConfigurationProvider : ConfigurationProvider
    {
        private readonly IAmazonSimpleSystemsManagement _client;
        private readonly string _path;
        private readonly IAwsParameterManager _parameterManager;

        public AwsParameterStoreConfigurationProvider(IAmazonSimpleSystemsManagement client, string path, IAwsParameterManager parameterManager)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (parameterManager == null)
            {
                throw new ArgumentNullException(nameof(parameterManager));
            }
            _client = client;
            _path = path;
            _parameterManager = parameterManager;
        }
        public override void Load() => LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        private async Task LoadAsync()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var response = await GetParametersAsync(_path);

            do
            {
                foreach (var parameter in response.Parameters)
                {
                    if (!_parameterManager.LoadParameter(parameter))
                    {
                        continue;
                    }
                    var key = _parameterManager.GetKey(parameter);
                    data.Add(key, parameter.Value);
                }

                response = response.NextToken != null ?
                                await GetParametersAsync(_path, response.NextToken)
                                : null;
            } while (response != null);

            Data = data;
        }
        private async Task<GetParametersByPathResponse> GetParametersAsync(string path, string nextToken = null)
        {
            var response = await _client.GetParametersByPathAsync(new GetParametersByPathRequest
            {
                WithDecryption = true,
                Recursive = true,
                Path = path,
                NextToken = nextToken
            });
            return response;
        }
    }
}
