namespace GroveGames.Platform;

public interface IDisplayDevice
{
    public bool HasInput { get; }
    public int TargetFrameRate { get; }

    public void SetTargetFrameRate(int frameRate);
    public void SetKeepScreenOn(bool keepScreenOn);
}
