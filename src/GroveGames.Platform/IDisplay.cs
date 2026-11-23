namespace GroveGames.Platform;

public interface IDisplay
{
    public int TargetFrameRate { get; }
    public SleepTimeout SleepTimeout { get; }

    public void SetTargetFrameRate(int frameRate);
    public void SetSleepTimeout(SleepTimeout sleepTimeout);
}
