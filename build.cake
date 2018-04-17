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
        DotNetCoreRestore();
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("RestoreNugetPackages")
    .Does(() =>
    {
        var solution = "./Configuration.AwsParameterStore.sln";
        Information("Building {0}", solution);
        DotNetCoreBuild(solution, new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            NoRestore = true
        });
    });

Task("RunUnitTests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        DotNetCoreTest("./Configuration.AwsParameterStore.Tests/Configuration.AwsParameterStore.Tests.csproj", new DotNetCoreTestSettings
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
        DotNetCorePack("./Configuration.AwsParameterStore/Configuration.AwsParameterStore.csproj", new DotNetCorePackSettings
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