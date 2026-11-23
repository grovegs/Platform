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

            var utcNow = new DateTime(GetInt64(UtcNowHighKey, UtcNowLowKey), DateTimeKind.Utc);
            var timeSinceBoot = new TimeSpan(GetInt64(TimeSinceBootHighKey, TimeSinceBootLowKey));
            anchor = new ClockAnchor(utcNow, timeSinceBoot, PlayerPrefs.GetString(BootIdKey));
            return true;
        }

        public void Save(ClockAnchor anchor)
        {
            SetInt64(UtcNowHighKey, UtcNowLowKey, anchor.UtcNow.Ticks);
            SetInt64(TimeSinceBootHighKey, TimeSinceBootLowKey, anchor.TimeSinceBoot.Ticks);
            PlayerPrefs.SetString(BootIdKey, anchor.BootId);
            PlayerPrefs.Save();
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
