namespace GroveGames.Platform;

public sealed class PlatformSettings : IPlatformSettings
{
    private readonly int _targetFrameRate;
    private readonly SleepTimeout _sleepTimeout;

    public int TargetFrameRate => _targetFrameRate;
    public SleepTimeout SleepTimeout => _sleepTimeout;

    public PlatformSettings(int targetFrameRate, SleepTimeout sleepTimeout)
    {
        _targetFrameRate = targetFrameRate;
        _sleepTimeout = sleepTimeout;
    }
}
