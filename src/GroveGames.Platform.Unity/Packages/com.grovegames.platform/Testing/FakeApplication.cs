using System;

namespace GroveGames.Platform.Testing
{
    public sealed class FakeApplication : IApplication
    {
        private ApplicationState _state = ApplicationState.Active;

        public event Action<ApplicationStateChangedEvent>? StateChanged;

        public ApplicationState State => _state;

        public void SetState(ApplicationState state)
        {
            var previous = _state;
            _state = state;
            StateChanged?.Invoke(new ApplicationStateChangedEvent(previous, state));
        }

        public void Quit()
        {
        }
    }
}
