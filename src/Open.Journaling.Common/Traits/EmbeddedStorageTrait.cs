using System;

namespace Open.Journaling.Traits
{
    public class EmbeddedStorageTrait
        : IJournalTrait
    {
        public EmbeddedStorageTrait(
            TriState value)
        {
            Value = value;
        }

        public TriState Value { get; }
    }
}