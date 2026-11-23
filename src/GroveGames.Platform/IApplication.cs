using System;

namespace GroveGames.Platform;

public interface IApplication
{
    public event Action<ApplicationStateChangedEvent>? StateChanged;

    public ApplicationState State { get; }

    public void Quit();
}
