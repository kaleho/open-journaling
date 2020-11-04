using System;

namespace Open.Journaling.Model
{
    public interface IJournalEntry
    {
        public static Type Type = typeof(IJournalEntry);

        string EntryId { get; }

        string JournalId { get; }

        object Meta { get; }

        object Payload { get; }

        long Sequence { get; }

        string[] Tags { get; }

        long UtcTicks { get; }

        void Deconstruct(
            out string journalId,
            out string entryId,
            out object meta,
            out object payload,
            out string[] tags,
            out long sequence,
            out long utcTicks)
        {
            journalId = JournalId;

            entryId = EntryId;

            meta = Meta;

            payload = Payload;

            tags = Tags;

            sequence = Sequence;

            utcTicks = UtcTicks;
        }
    }
}