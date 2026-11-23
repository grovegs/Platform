namespace GroveGames.Platform;

public interface IPlatformSettings
{
    public int TargetFrameRate { get; }
    public SleepTimeout SleepTimeout { get; }
}
