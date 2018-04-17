using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;

namespace Configuration.AwsParameterStore
{
    public class DefaultAwsParameterManager : IAwsParameterManager
    {
        public virtual string GetKey(Parameter parameter)
        {
            return parameter.Name.TrimStart('/').Replace("/", ConfigurationPath.KeyDelimiter);
        }

        public virtual bool LoadParameter(Parameter parameter)
        {
            return true;
        }
    }
}
