using System;

namespace Open.Journaling.Traits
{
    public class AtomicTrait
        : IJournalTrait
    {
        public AtomicTrait(
            TriState value)
        {
            Value = value;
        }

        public TriState Value { get; }
    }
}