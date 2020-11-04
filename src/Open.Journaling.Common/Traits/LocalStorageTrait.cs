using System;

namespace Open.Journaling.Traits
{
    public class LocalStorageTrait
        : IJournalTrait
    {
        public LocalStorageTrait(
            TriState value)
        {
            Value = value;
        }

        public TriState Value { get; }
    }
}