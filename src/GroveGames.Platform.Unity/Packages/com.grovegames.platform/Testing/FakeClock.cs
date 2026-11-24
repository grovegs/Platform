using System;

namespace GroveGames.Platform.Testing
{
    public sealed class FakeClock : IClock
    {
        private DateTime _utcNow = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        public DateTime UtcNow => _utcNow;

        public DateTime GetDeviceTime(DateTime utcTime)
        {
            return utcTime;
        }

        public void Advance(TimeSpan duration)
        {
            _utcNow += duration;
        }
    }
}
