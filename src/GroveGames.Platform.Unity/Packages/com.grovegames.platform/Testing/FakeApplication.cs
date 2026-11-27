using System;

namespace GroveGames.Platform.Testing
{
    public sealed class FakeApplication : IApplication
    {
        private ApplicationState _state;

        public event Action<ApplicationStateChangedEvent>? StateChanged;

        public ApplicationState State => _state;

        public FakeApplication()
        {
            _state = ApplicationState.Active;
        }

        public void SetState(ApplicationState state)
        {
            if (state == _state)
            {
                return;
            }

            var previous = _state;
            _state = state;
            StateChanged?.Invoke(new ApplicationStateChangedEvent(previous, state));
        }

        public void Quit()
        {
        }
    }
}
