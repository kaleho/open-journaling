using System;

namespace Open.Journaling.Traits
{
    public class DurableTrait
        : IJournalTrait
    {
        public DurableTrait(
            TriState value)
        {
            Value = value;
        }

        public TriState Value { get; }
    }
}