using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Configuration.AwsParameterStore.Tests
{
    public class AwsParameterStoreConfigurationProviderTests
    {
        const string ValidPath = "/";

        [Fact]
        public void ConstructorShouldThrowsForNullClient()
        {
            Assert.Throws<ArgumentNullException>("client", () =>
                new AwsParameterStoreConfigurationProvider(
                    null,
                    ValidPath,
                    Substitute.For<IAwsParameterManager>()));
        }
        [Fact]
        public void ConstructorShouldThrowsForNullPath()
        {
            Assert.Throws<ArgumentNullException>("path", () =>
                new AwsParameterStoreConfigurationProvider(
                    Substitute.For<IAmazonSimpleSystemsManagement>(),
                    null,
                    Substitute.For<IAwsParameterManager>()));
        }
        [Fact]
        public void ConstructorShouldThrowsForNullManager()
        {
            Assert.Throws<ArgumentNullException>("parameterManager", () =>
                new AwsParameterStoreConfigurationProvider(
                    Substitute.For<IAmazonSimpleSystemsManagement>(),
                    ValidPath,
                    null));
        }
        [Fact]
        public void ShouldLoadAllParameters()
        {
            var client = Substitute.For<IAmazonSimpleSystemsManagement>();
            client.GetParametersByPathAsync(Arg.Is<GetParametersByPathRequest>(x => x.NextToken == null))
                .Returns(new GetParametersByPathResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK,
                    NextToken = "nexttoken",
                    Parameters = new List<Parameter>
                    {
                        new Parameter
                        {
                            Name="parameter1",
                            Type = ParameterType.String,
                            Value = "value1"
                        },

                    }
                });

            client.GetParametersByPathAsync(Arg.Is<GetParametersByPathRequest>(x => x.NextToken == "nexttoken"))
                .Returns(new GetParametersByPathResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK,
                    NextToken = null,
                    Parameters = new List<Parameter>
                    {
                        new Parameter
                        {
                            Name="parameter2",
                            Type = ParameterType.String,
                            Value = "value2"
                        },

                    }
                });


            var provider = new AwsParameterStoreConfigurationProvider(client, ValidPath, new DefaultAwsParameterManager());
            provider.Load();

            var keys = provider.GetChildKeys(Enumerable.Empty<string>(), null).ToArray();
            Assert.Equal(new[] { "parameter1", "parameter2" }, keys);

            provider.ShouldHaveKeyWithValue("parameter1", "value1");
            provider.ShouldHaveKeyWithValue("parameter2", "value2");
        }

        [Fact]
        public void ShouldNotLoadFilteredItems()
        {
            var client = Substitute.For<IAmazonSimpleSystemsManagement>();
            client.GetParametersByPathAsync(Arg.Is<GetParametersByPathRequest>(x => x.NextToken == null))
                .Returns(new GetParametersByPathResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK,
                    NextToken = "nexttoken",
                    Parameters = new List<Parameter>
                    {
                        new Parameter
                        {
                            Name="parameter1",
                            Type = ParameterType.String,
                            Value = "value1"
                        },
                        new Parameter
                        {
                            Name="filteredparameter",
                            Type = ParameterType.String,
                            Value = "value"
                        }

                    }
                });

            var provider = new AwsParameterStoreConfigurationProvider(client, ValidPath, new StartsWithFilteredParameterManager());
            provider.Load();

            var keys = provider.GetChildKeys(Enumerable.Empty<string>(), null).ToArray();
            Assert.Equal(new[] { "parameter1" }, keys);
            provider.ShouldHaveKeyWithValue("parameter1", "value1");
        }
        [Fact]
        public void ShouldOverrideExistingKeyWhenReload()
        {
            var client = Substitute.For<IAmazonSimpleSystemsManagement>();
            var parameter = new Parameter
            {
                Name = "parameter1",
                Type = ParameterType.String,
                Value = "value1"
            };
            var response = new GetParametersByPathResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                NextToken = "nexttoken",
                Parameters = new List<Parameter>
                {
                    parameter
                }
            };
            client.GetParametersByPathAsync(Arg.Is<GetParametersByPathRequest>(x => x.NextToken == null)).Returns(response);

            var provider = new AwsParameterStoreConfigurationProvider(client, ValidPath, new StartsWithFilteredParameterManager());
            provider.Load();

            provider.ShouldHaveKeyWithValue("parameter1", "value1");
            parameter.Value = "value2";

            provider.Load();
            provider.ShouldHaveKeyWithValue("parameter1", "value2");
        }
        [Fact]
        public void ShouldReplaceForwardSlashInParameterName()
        {
            var client = Substitute.For<IAmazonSimpleSystemsManagement>();
            client.GetParametersByPathAsync(Arg.Is<GetParametersByPathRequest>(x => x.NextToken == null))
                .Returns(new GetParametersByPathResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK,
                    NextToken = null,
                    Parameters = new List<Parameter>
                    {
                        new Parameter
                        {
                            Name="/section1/parameter1",
                            Type = ParameterType.String,
                            Value = "value1"
                        }

                    }
                });

            var provider = new AwsParameterStoreConfigurationProvider(client, ValidPath, new StartsWithFilteredParameterManager());
            provider.Load();

            provider.ShouldHaveKeyWithValue("section1:parameter1", "value1");
        }


        private class StartsWithFilteredParameterManager : DefaultAwsParameterManager
        {
            public override bool LoadParameter(Parameter parameter)
            {
                return !parameter.Name.StartsWith("filtered");
            }
        }
    }
}
