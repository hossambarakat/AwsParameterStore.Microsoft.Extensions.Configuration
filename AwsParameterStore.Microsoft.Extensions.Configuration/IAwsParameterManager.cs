using Amazon.SimpleSystemsManagement.Model;

namespace AwsParameterStore.Microsoft.Extensions.Configuration
{
    public interface IAwsParameterManager
    {
        string GetKey(Parameter parameter);
        bool LoadParameter(Parameter parameter);
    }
}
