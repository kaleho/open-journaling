using System.Collections.Generic;
using Open.Journaling.Traits;

namespace Open.Journaling
{
    public interface IJournalReaderProvider
        : IJournalProvider
    {
        bool TryGetOrCreate(
            JournalId journalId,
            IEnumerable<IJournalTrait> traits,
            out IJournalReader reader);
    }
}