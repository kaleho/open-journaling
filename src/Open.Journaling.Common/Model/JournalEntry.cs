using System;

namespace Open.Journaling.Model
{
    public class JournalEntry
        : IJournalEntry
    {
        public static Type Type = typeof(JournalEntry);

        public JournalEntry(
            string journalId,
            string entryId,
            long sequence,
            long utcTicks,
            object meta,
            object payload,
            params string[] tags)
        {
            JournalId = journalId;
            EntryId = entryId;
            Meta = meta;
            Payload = payload;
            Tags = tags;
            Sequence = sequence;
            UtcTicks = utcTicks;
        }

        private JournalEntry()
        {
        }

        public string EntryId { get; }

        public string JournalId { get; }

        public object Meta { get; }

        public object Payload { get; }

        public long Sequence { get; }

        public string[] Tags { get; }

        public long UtcTicks { get; }
    }
}