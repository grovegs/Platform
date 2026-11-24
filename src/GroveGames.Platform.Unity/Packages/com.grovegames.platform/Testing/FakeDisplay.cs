namespace GroveGames.Platform.Testing
{
    public sealed class FakeDisplay : IDisplay
    {
        private int _targetFrameRate = -1;
        private SleepTimeout _sleepTimeout = SleepTimeout.SystemSetting;

        public int TargetFrameRate => _targetFrameRate;
        public SleepTimeout SleepTimeout => _sleepTimeout;

        public void SetTargetFrameRate(int frameRate)
        {
            _targetFrameRate = frameRate;
        }

        public void SetSleepTimeout(SleepTimeout sleepTimeout)
        {
            _sleepTimeout = sleepTimeout;
        }
    }
}
