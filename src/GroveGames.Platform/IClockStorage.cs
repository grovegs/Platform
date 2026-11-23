namespace GroveGames.Platform;

public interface IClockStorage
{
    public bool TryLoad(out ClockAnchor anchor);
    public void Save(ClockAnchor anchor);
}
