using Amazon.SimpleSystemsManagement.Model;

namespace Configuration.AwsParameterStore
{
    public interface IAwsParameterManager
    {
        string GetKey(Parameter parameter);
        bool LoadParameter(Parameter parameter);
    }
}
