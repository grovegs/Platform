#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GroveGames.Platform.Unity
{
    internal sealed class IosClockDevice : IClockDevice
    {
        private const ulong NanosecondsPerTick = 100UL;
        private const int BootIdCapacity = 64;

        [DllImport("__Internal")]
        private static extern ulong GroveGamesPlatform_GetTimeSinceBootNanoseconds();

        [DllImport("__Internal")]
        private static extern int GroveGamesPlatform_GetBootId([Out] byte[] buffer, int capacity);

        private readonly string _bootId;

        public DateTime UtcNow => DateTime.UtcNow;
        public TimeSpan TimeSinceBoot => new((long)(GroveGamesPlatform_GetTimeSinceBootNanoseconds() / NanosecondsPerTick));
        public string BootId => _bootId;

        public IosClockDevice()
        {
            var buffer = new byte[BootIdCapacity];
            var length = GroveGamesPlatform_GetBootId(buffer, buffer.Length);
            _bootId = Encoding.ASCII.GetString(buffer, 0, length);

            if (_bootId.Length == 0)
            {
                UnityEngine.Debug.LogWarning("The boot session id is unavailable, so moving the device clock while the game is closed changes the game clock.");
            }
        }
    }
}
#endif
