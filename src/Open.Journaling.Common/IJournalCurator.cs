using System;
using System.Collections.Generic;
using Open.Journaling.Traits;

namespace Open.Journaling
{
    public interface IJournalCurator
    {
        static readonly IEnumerable<IJournalTrait> DefaultJournalTraits =
            new IJournalTrait[]
            {
                new AtomicTrait(TriState.True),
                new DurableTrait(TriState.True)
            };
        static readonly IEnumerable<IJournalTrait> DefaultProviderTraits =
            new IJournalTrait[]
            {
                new LocalStorageTrait(TriState.True)
            };

        bool Exists(
            JournalId journalId);

        bool TryGetOrCreateReader(
            JournalId journalId,
            IEnumerable<IJournalTrait> providerTraits,
            IEnumerable<IJournalTrait> journalTraits,
            out IJournalReader reader);

        /// <summary>
        ///     Get or create reader using the Uri of
        ///     the JournalId to determine the
        ///     <see cref="LocalStorageTrait" /> or
        ///     <see cref="RemoteStorageTrait" /> <see cref="IJournalTrait" />
        /// </summary>
        /// <param name="journalId"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        bool TryGetOrCreateReader(
            JournalId journalId,
            out IJournalReader reader);

        bool TryGetOrCreateReader(
            JournalId journalId,
            Func<IEnumerable<IJournalReaderProvider>, IJournalReaderProvider> providerSelector,
            out IJournalReader reader,
            IEnumerable<IJournalTrait> journalTraits = default);

        bool TryGetOrCreateReader(
            string connectionString,
            out IJournalReader reader);

        bool TryGetOrCreateWriter(
            JournalId journalId,
            IEnumerable<IJournalTrait> providerTraits,
            IEnumerable<IJournalTrait> journalTraits,
            out IJournalWriter writer);

        /// <summary>
        ///     Get or create writer using the Uri of
        ///     the JournalId to determine the
        ///     <see cref="LocalStorageTrait" /> or
        ///     <see cref="RemoteStorageTrait" /> <see cref="IJournalTrait" />
        /// </summary>
        /// <param name="journalId"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        bool TryGetOrCreateWriter(
            JournalId journalId,
            out IJournalWriter writer);

        bool TryGetOrCreateWriter(
            JournalId journalId,
            Func<IEnumerable<IJournalWriterProvider>, IJournalWriterProvider> providerSelector,
            out IJournalWriter writer,
            IEnumerable<IJournalTrait> journalTraits = default);

        bool TryGetOrCreateWriter(
            string connectionString,
            out IJournalWriter writer);
    }
}