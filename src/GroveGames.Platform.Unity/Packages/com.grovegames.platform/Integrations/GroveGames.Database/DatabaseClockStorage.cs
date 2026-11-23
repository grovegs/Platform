using GroveGames.Database;

namespace GroveGames.Platform.Unity
{
    public sealed class DatabaseClockStorage : IClockStorage
    {
        public const string DocumentName = "clock";

        private readonly IDatabase _database;

        private IDocument<ClockSnapshot>? _document;

        private IDocument<ClockSnapshot> Document => _document ??= _database.GetDocument<ClockSnapshot>(DocumentName);

        public DatabaseClockStorage(IDatabase database)
        {
            _database = database;
        }

        public bool TryLoad(out ClockAnchor anchor)
        {
            var snapshot = Document.Value;

            if (snapshot.UtcNow == default && snapshot.BootId.Length == 0)
            {
                anchor = default;
                return false;
            }

            anchor = new ClockAnchor(snapshot.UtcNow, snapshot.TimeSinceBoot, snapshot.BootId);
            return true;
        }

        public void Save(ClockAnchor anchor)
        {
            var snapshot = Document.Value;
            snapshot.UtcNow = anchor.UtcNow;
            snapshot.TimeSinceBoot = anchor.TimeSinceBoot;
            snapshot.BootId = anchor.BootId;
            Document.Set(snapshot);
        }
    }
}
