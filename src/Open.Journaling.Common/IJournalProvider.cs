using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Open.Journaling.Traits;

namespace Open.Journaling
{
    public interface IJournalProvider
    {
        ImmutableDictionary<JournalId, Lazy<IJournal>> Cache { get; }

        string ConnectionString { get; }

        ProviderId ProviderId { get; }

        JournalTraits Traits { get; }

        bool HasJournal(
            JournalId journalId);

        bool HasTraits(
            IEnumerable<IJournalTrait> traits);

        bool OwnsConnection(
            string connectionString);
    }
}