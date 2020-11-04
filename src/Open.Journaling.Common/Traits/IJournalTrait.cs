using System;

namespace Open.Journaling.Traits
{
    public interface IJournalTrait
    {
        TriState Value { get; }
    }
}