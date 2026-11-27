using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GroveGames.Platform.Unity.Editor.Tests
{
    public sealed class PlayerPrefsClockStorageTests
    {
        private static readonly string[] s_keys =
        {
            PlayerPrefsClockStorage.KeyPrefix + "UtcNow.0",
            PlayerPrefsClockStorage.KeyPrefix + "UtcNow.1",
            PlayerPrefsClockStorage.KeyPrefix + "TimeSinceBoot.0",
            PlayerPrefsClockStorage.KeyPrefix + "TimeSinceBoot.1",
            PlayerPrefsClockStorage.KeyPrefix + "BootId"
        };

        [SetUp]
        public void SetUp()
        {
            DeleteKeys();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteKeys();
        }

        [Test]
        public void TryLoad_NothingSaved_ReturnsFalse()
        {
            var storage = new PlayerPrefsClockStorage();

            var loaded = storage.TryLoad(out _);

            Assert.That(loaded, Is.False);
        }

        [Test]
        public void Save_Anchor_NewStorageLoadsItExactly()
        {
            var expected = new ClockAnchor(new DateTime(2026, 1, 1, 12, 0, 0, 123, DateTimeKind.Utc), TimeSpan.FromTicks(98765432109876L), "boot-1");
            new PlayerPrefsClockStorage().Save(expected);

            var loaded = new PlayerPrefsClockStorage().TryLoad(out var anchor);

            Assert.That(loaded, Is.True);
            Assert.That(anchor.UtcNow, Is.EqualTo(expected.UtcNow));
            Assert.That(anchor.UtcNow.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(anchor.TimeSinceBoot, Is.EqualTo(expected.TimeSinceBoot));
            Assert.That(anchor.BootId, Is.EqualTo(expected.BootId));
        }

        [Test]
        public void TryLoad_EditedTicks_ReturnsFalseWithAWarning()
        {
            new PlayerPrefsClockStorage().Save(new ClockAnchor(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeSpan.FromHours(1d), "boot-1"));
            PlayerPrefs.SetInt(s_keys[0], -1);

            LogAssert.Expect(LogType.Warning, "The saved clock state is invalid, so the clock starts from the device time.");
            var loaded = new PlayerPrefsClockStorage().TryLoad(out _);

            Assert.That(loaded, Is.False);
        }

        private static void DeleteKeys()
        {
            foreach (var key in s_keys)
            {
                PlayerPrefs.DeleteKey(key);
            }

            PlayerPrefs.Save();
        }
    }
}
