namespace Open.Journaling.Model
{
    public class SerializedEntry
        : ISerializedEntry
    {
        public SerializedEntry(
            string entryId,
            object meta,
            object payload,
            params string[] tags)
        {
            EntryId = entryId;
            Meta = meta;
            Payload = payload;
            Tags = tags;
        }

        private SerializedEntry()
        {
        }

        public string EntryId { get; }

        public object Meta { get; }

        public object Payload { get; }

        public string[] Tags { get; }
    }
}