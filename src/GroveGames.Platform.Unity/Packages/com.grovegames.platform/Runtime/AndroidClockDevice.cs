#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GroveGames.Platform.Unity
{
    internal sealed class AndroidClockDevice : IClockDevice
    {
        private const int BootTimeClockId = 7;
        private const long NanosecondsPerTick = 100L;
        private const string BootCountKey = "boot_count";

        [DllImport("libc", EntryPoint = "clock_gettime")]
        private static extern int GetClockTime(int clockId, [Out] IntPtr[] time);

        private readonly IntPtr[] _time;
        private readonly string _bootId;
        private TimeSpan _timeSinceBoot;

        public DateTime UtcNow => DateTime.UtcNow;
        public string BootId => _bootId;

        public TimeSpan TimeSinceBoot
        {
            get
            {
                if (GetClockTime(BootTimeClockId, _time) == 0)
                {
                    _timeSinceBoot = new TimeSpan(_time[0].ToInt64() * TimeSpan.TicksPerSecond + _time[1].ToInt64() / NanosecondsPerTick);
                }

                return _timeSinceBoot;
            }
        }

        public AndroidClockDevice()
        {
            _time = new IntPtr[2];
            _bootId = GetBootId();
        }

        private static string GetBootId()
        {
            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                if (activity == null)
                {
                    return string.Empty;
                }

                using var contentResolver = activity.Call<AndroidJavaObject>("getContentResolver");
                using var settings = new AndroidJavaClass("android.provider.Settings$Global");
                var bootCount = settings.CallStatic<int>("getInt", contentResolver, BootCountKey, 0);
                return bootCount > 0 ? bootCount.ToString() : string.Empty;
            }
            catch (AndroidJavaException)
            {
                return string.Empty;
            }
        }
    }
}
#endif
