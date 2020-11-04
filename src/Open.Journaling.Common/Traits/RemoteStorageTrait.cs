using System;

namespace Open.Journaling.Traits
{
    public class RemoteStorageTrait
        : IJournalTrait
    {
        public RemoteStorageTrait(
            TriState value)
        {
            Value = value;
        }

        public TriState Value { get; }
    }
}