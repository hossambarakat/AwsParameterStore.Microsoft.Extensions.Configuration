///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////
var artifactDirectory = "./artifacts/";
Task("Clean")
    .Does(() =>
    {
        Information("Cleaning artifacts directory");
        CleanDirectory(artifactDirectory);
    });

Task("RestoreNugetPackages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetRestore();
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("RestoreNugetPackages")
    .Does(() =>
    {
        var solution = "./AwsParameterStore.Microsoft.Extensions.Configuration.sln";
        Information("Building {0}", solution);
        DotNetBuild(solution, new DotNetBuildSettings
        {
            Configuration = configuration,
            NoRestore = true
        });
    });

Task("RunUnitTests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        DotNetTest("./AwsParameterStore.Microsoft.Extensions.Configuration.Tests/AwsParameterStore.Microsoft.Extensions.Configuration.Tests.csproj", new DotNetTestSettings
        {
            Configuration=configuration,
            NoBuild=true,
            NoRestore = true
        });
    });

Task("Package")
    .IsDependentOn("Build")
    .IsDependentOn("RunUnitTests")
    .Does(() =>
    {
        DotNetPack("./AwsParameterStore.Microsoft.Extensions.Configuration/AwsParameterStore.Microsoft.Extensions.Configuration.csproj", new DotNetPackSettings
        {
            Configuration=configuration,
            OutputDirectory=artifactDirectory,
            NoBuild= true
        });
    });

Task("Default")
    .IsDependentOn("Package")
    .Does(() => {

    });

RunTarget(target);