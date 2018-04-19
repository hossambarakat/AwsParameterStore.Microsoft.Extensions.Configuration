Microsoft.Extensions.Configuration.AwsParameterStore
===============

[![Build Status](https://ci.appveyor.com/api/projects/status/loosxok30ptwjeei?svg=true)](https://ci.appveyor.com/project/HossamBarakat/configuration-awsparameterstore)

 [AWS Systems Manager Parameter Store](https://docs.aws.amazon.com/systems-manager/latest/userguide/systems-manager-paramstore.html) configuration provider implementation for [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/index?view=aspnetcore-2.1&tabs=basicconfiguration).

## Getting Started

You should install Microsoft.Extensions.Configuration.AwsParameterStore with [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.AwsParameterStore):

    Install-Package Microsoft.Extensions.Configuration.AwsParameterStore

Or via the .NET Core command line interface:

    dotnet add package Microsoft.Extensions.Configuration.AwsParameterStore

The provider is added to the `ConfigurationBuilder` using the `AddAwsParameterStore ` extension. The default method accepts one parameter `path` which is the prefix of the parameter store hierarchy.

```csharp
    configrationBuilder.AddAwsParameterStore("/prefix");
```

## Hierarchical Parameters Names Mapping
Parameter Store supports parameter hierarchy. The Hierarchical values use "/" as a separator such as "/DeploymentConfig/Prod/FleetHealth". ASP.NET Core configuration normally uses colon as separator so before adding the values to the configuration we swap the forward slash "/" with colon.

## Credentials
By default, AWS Access Key ID and AWS Secret Access Key are discovered from environment variables `AWS_ACCESS_KEY_ID` and `AWS_SECRET_ACCESS_KEY` respectively.

## Required Permissions
An AWS IAM account with `ssm:GetParametersByPath` permission.

## Reloading secrets
Secrets are cached until IConfigurationRoot.Reload() is called. Expired, disabled, and updated secrets in the key vault are not respected by the application until Reload is executed.

```csharp
    Configuration.Reload();
```

## Additional resources
- [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/index?view=aspnetcore-2.1&tabs=basicconfiguration)
- [AWS Systems Manager Parameter Store Documentation](https://docs.aws.amazon.com/systems-manager/latest/userguide/systems-manager-paramstore.html)
- [GetParametersByPath API](https://docs.aws.amazon.com/systems-manager/latest/APIReference/API_GetParametersByPath.html)