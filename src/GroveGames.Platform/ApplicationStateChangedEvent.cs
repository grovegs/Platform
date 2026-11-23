namespace GroveGames.Platform;

public readonly struct ApplicationStateChangedEvent
{
    private readonly ApplicationState _previous;
    private readonly ApplicationState _current;

    public ApplicationState Previous => _previous;
    public ApplicationState Current => _current;

    public ApplicationStateChangedEvent(ApplicationState previous, ApplicationState current)
    {
        _previous = previous;
        _current = current;
    }
}
