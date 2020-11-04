using System.Collections.Generic;
using Open.Journaling.Traits;

namespace Open.Journaling
{
    public interface IJournalWriterProvider
        : IJournalProvider
    {
        bool TryGetOrCreate(
            JournalId journalId,
            IEnumerable<IJournalTrait> traits,
            out IJournalWriter writer);
    }
}