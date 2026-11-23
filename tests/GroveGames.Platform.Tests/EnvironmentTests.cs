namespace GroveGames.Platform.Tests;

public sealed class EnvironmentTests
{
    [Theory]
    [InlineData(EnvironmentType.Editor, true, false, false)]
    [InlineData(EnvironmentType.Development, false, true, false)]
    [InlineData(EnvironmentType.Production, false, false, true)]
    public void Constructor_Type_SetsOneFlag(EnvironmentType type, bool isEditor, bool isDevelopment, bool isProduction)
    {
        var environment = new Environment(type, "1.2.3");

        Assert.Equal(type, environment.Type);
        Assert.Equal(isEditor, environment.IsEditor);
        Assert.Equal(isDevelopment, environment.IsDevelopment);
        Assert.Equal(isProduction, environment.IsProduction);
    }

    [Theory]
    [InlineData(EnvironmentType.Editor, "1.2.3-editor")]
    [InlineData(EnvironmentType.Development, "1.2.3-dev")]
    [InlineData(EnvironmentType.Production, "1.2.3")]
    public void VersionName_Type_AddsItsSuffix(EnvironmentType type, string expected)
    {
        var environment = new Environment(type, "1.2.3");

        Assert.Equal(new Version(1, 2, 3), environment.Version);
        Assert.Equal(expected, environment.VersionName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not a version")]
    public void Constructor_VersionCannotBeParsed_UsesZero(string? version)
    {
        var environment = new Environment(EnvironmentType.Production, version);

        Assert.Equal(new Version(0, 0), environment.Version);
        Assert.Equal("0.0", environment.VersionName);
    }
}
