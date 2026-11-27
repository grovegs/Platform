using System;
using UnityEngine;

namespace GroveGames.Platform.Unity
{
    public sealed class PlayerPrefsClockStorage : IClockStorage
    {
        public const string KeyPrefix = "GroveGames.Platform.Clock.";

        private const string UtcNowHighKey = KeyPrefix + "UtcNow.0";
        private const string UtcNowLowKey = KeyPrefix + "UtcNow.1";
        private const string TimeSinceBootHighKey = KeyPrefix + "TimeSinceBoot.0";
        private const string TimeSinceBootLowKey = KeyPrefix + "TimeSinceBoot.1";
        private const string BootIdKey = KeyPrefix + "BootId";

        public bool TryLoad(out ClockAnchor anchor)
        {
            if (!PlayerPrefs.HasKey(BootIdKey))
            {
                anchor = default;
                return false;
            }

            var utcNowTicks = GetInt64(UtcNowHighKey, UtcNowLowKey);
            var timeSinceBootTicks = GetInt64(TimeSinceBootHighKey, TimeSinceBootLowKey);

            if (utcNowTicks < DateTime.MinValue.Ticks || utcNowTicks > DateTime.MaxValue.Ticks || timeSinceBootTicks < 0L)
            {
                Debug.LogWarning("The saved clock state is invalid, so the clock starts from the device time.");
                anchor = default;
                return false;
            }

            anchor = new ClockAnchor(new DateTime(utcNowTicks, DateTimeKind.Utc), new TimeSpan(timeSinceBootTicks), PlayerPrefs.GetString(BootIdKey));
            return true;
        }

        public void Save(ClockAnchor anchor)
        {
            try
            {
                SetInt64(UtcNowHighKey, UtcNowLowKey, anchor.UtcNow.Ticks);
                SetInt64(TimeSinceBootHighKey, TimeSinceBootLowKey, anchor.TimeSinceBoot.Ticks);
                PlayerPrefs.SetString(BootIdKey, anchor.BootId);
                PlayerPrefs.Save();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private static long GetInt64(string highKey, string lowKey)
        {
            return ((long)PlayerPrefs.GetInt(highKey) << 32) | (uint)PlayerPrefs.GetInt(lowKey);
        }

        private static void SetInt64(string highKey, string lowKey, long value)
        {
            PlayerPrefs.SetInt(highKey, (int)(value >> 32));
            PlayerPrefs.SetInt(lowKey, (int)value);
        }
    }
}
