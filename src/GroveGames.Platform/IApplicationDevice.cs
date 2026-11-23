namespace GroveGames.Platform;

public interface IApplicationDevice
{
    public event Action<bool>? PauseChanged;
    public event Action<bool>? FocusChanged;

    public bool IsFocused { get; }

    public void Quit();
}
