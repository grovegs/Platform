namespace GroveGames.Platform;

public sealed class Environment
{
    private readonly EnvironmentType _type;
    private readonly Version _version;
    private readonly string _versionName;

    public EnvironmentType Type => _type;
    public bool IsEditor => _type == EnvironmentType.Editor;
    public bool IsDevelopment => _type == EnvironmentType.Development;
    public bool IsProduction => _type == EnvironmentType.Production;
    public Version Version => _version;
    public string VersionName => _versionName;

    public Environment(EnvironmentType type, string? version)
    {
        _type = type;
        _version = Version.TryParse(version, out var parsed) ? parsed : new Version(0, 0);
        _versionName = type switch
        {
            EnvironmentType.Editor => $"{_version}-editor",
            EnvironmentType.Development => $"{_version}-dev",
            _ => _version.ToString()
        };
    }
}
