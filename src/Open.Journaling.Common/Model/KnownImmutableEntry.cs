namespace Open.Journaling.Model
{
    public class KnownImmutableEntry
        : IKnownImmutableEntry
    {
        public KnownImmutableEntry(
            string entryId,
            object meta,
            object payload,
            long version,
            params string[] tags)
        {
            EntryId = entryId;
            Meta = meta;
            Payload = payload;
            Version = version;
            Tags = tags;
        }

        public string EntryId { get; }

        public object Meta { get; }

        public object Payload { get; }

        public string[] Tags { get; }

        public long Version { get; }
    }
}