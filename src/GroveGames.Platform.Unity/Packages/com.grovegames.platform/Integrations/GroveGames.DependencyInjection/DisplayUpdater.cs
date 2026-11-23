using GroveGames.DependencyInjection;

namespace GroveGames.Platform.Unity
{
    internal sealed class DisplayUpdater : IFrameUpdatable
    {
        private readonly Display _target;

        public DisplayUpdater(Display target)
        {
            _target = target;
        }

        public void FrameUpdate(float deltaTime)
        {
            _target.Update(UnityEngine.Time.unscaledDeltaTime);
        }
    }
}
