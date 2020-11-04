using System;

namespace Open.Journaling.Model
{
    public interface ISerializedEntry
    {
        public static Type Type = typeof(ISerializedEntry);

        string EntryId { get; }

        string[] Tags { get; }

        object Meta { get; }

        object Payload { get; }

        void Deconstruct(
            out string entryId,
            out object meta,
            out object payload,
            out string[] tags)
        {
            entryId = EntryId;

            meta = Meta;

            payload = Payload;

            tags = Tags;
        }
    }
}