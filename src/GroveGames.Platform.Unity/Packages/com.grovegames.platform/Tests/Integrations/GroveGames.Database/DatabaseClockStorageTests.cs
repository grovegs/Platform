using System;
using System.IO;
using GroveGames.Database;
using NUnit.Framework;

namespace GroveGames.Platform.Unity.Database.Editor.Tests
{
    public sealed class DatabaseClockStorageTests
    {
        private string _directory = null!;

        [SetUp]
        public void SetUp()
        {
            _directory = Path.Combine(Path.GetTempPath(), "DatabaseClockStorageTests", Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        [Test]
        public void TryLoad_NothingSaved_ReturnsFalse()
        {
            using var database = new FileDatabase(_directory);
            var storage = new DatabaseClockStorage(database);

            var loaded = storage.TryLoad(out _);

            Assert.That(loaded, Is.False);
        }

        [Test]
        public void Save_Anchor_ReopenedDatabaseLoadsIt()
        {
            var expected = new ClockAnchor(new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc), TimeSpan.FromHours(5d), "boot-1");

            using (var database = new FileDatabase(_directory))
            {
                new DatabaseClockStorage(database).Save(expected);
            }

            using var reopened = new FileDatabase(_directory);
            var loaded = new DatabaseClockStorage(reopened).TryLoad(out var anchor);

            Assert.That(loaded, Is.True);
            Assert.That(anchor.UtcNow, Is.EqualTo(expected.UtcNow));
            Assert.That(anchor.TimeSinceBoot, Is.EqualTo(expected.TimeSinceBoot));
            Assert.That(anchor.BootId, Is.EqualTo(expected.BootId));
        }
    }
}
