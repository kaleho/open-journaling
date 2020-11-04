using Open.Journaling.Traits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Open.Journaling
{
    public class DefaultJournalCurator
        : IJournalCurator
    {
        private readonly IEnumerable<IJournalReaderProvider> _readerProviders;
        private readonly IEnumerable<IJournalWriterProvider> _writerProviders;

        public DefaultJournalCurator(
            IEnumerable<IJournalReaderProvider> readerProviders,
            IEnumerable<IJournalWriterProvider> writerProviders)
        {
            _readerProviders = readerProviders;
            _writerProviders = writerProviders;
        }

        public bool Exists(
            JournalId journalId)
        {
            var returnValue =
                _readerProviders.Any(x => x.HasJournal(journalId)) ||
                _writerProviders.Any(x => x.HasJournal(journalId));

            return returnValue;
        }

        public bool TryGetOrCreateReader(
            JournalId journalId,
            IEnumerable<IJournalTrait> providerTraits,
            IEnumerable<IJournalTrait> journalTraits,
            out IJournalReader reader)
        {
            reader = null;

            var provider =
                _readerProviders.FirstOrDefault(x => x.HasJournal(journalId)) ??
                _readerProviders.FirstOrDefault(x => x.HasTraits(providerTraits));

            var hasReader =
                true ==
                provider?.TryGetOrCreate(
                    journalId,
                    journalTraits,
                    out reader);

            return hasReader;
        }

        public bool TryGetOrCreateReader(
            JournalId journalId,
            out IJournalReader reader)
        {
            var providerTraits =
                new[]
                {
                    new LocalStorageTrait(TriState.True)
                };

            return
                TryGetOrCreateReader(
                    journalId,
                    providerTraits,
                    IJournalCurator.DefaultJournalTraits,
                    out reader);
        }

        public bool TryGetOrCreateReader(
            JournalId journalId,
            Func<IEnumerable<IJournalReaderProvider>, IJournalReaderProvider> providerSelector,
            out IJournalReader reader,
            IEnumerable<IJournalTrait> journalTraits = default)
        {
            reader = null;

            var provider =
                _readerProviders.FirstOrDefault(x => x.HasJournal(journalId)) ??
                providerSelector(_readerProviders);

            var hasReader =
                true ==
                provider?.TryGetOrCreate(
                    journalId,
                    journalTraits ?? IJournalCurator.DefaultJournalTraits,
                    out reader);

            return hasReader;
        }

        public bool TryGetOrCreateReader(
            string connectionString,
            out IJournalReader reader)
        {
            reader = null;

            var parsed = new JournalConnectionString(connectionString);

            var journalId =
                parsed.TryGetKey("JournalId", out var actualKey)
                    ? new JournalId(parsed[actualKey])
                    : default;

            var provider =
                _readerProviders.FirstOrDefault(x => journalId != default && x.HasJournal(journalId)) ??
                _readerProviders.FirstOrDefault(x => x.OwnsConnection(connectionString));

            var hasReader =
                true ==
                provider?.TryGetOrCreate(
                    journalId,
                    IJournalCurator.DefaultJournalTraits,
                    out reader);

            return hasReader;
        }

        public bool TryGetOrCreateWriter(
            JournalId journalId,
            IEnumerable<IJournalTrait> providerTraits,
            IEnumerable<IJournalTrait> journalTraits,
            out IJournalWriter writer)
        {
            writer = null;

            var provider =
                _writerProviders.FirstOrDefault(x => x.HasJournal(journalId)) ??
                _writerProviders.FirstOrDefault(x => x.HasTraits(providerTraits));

            var hasWriter =
                true ==
                provider?.TryGetOrCreate(
                    journalId,
                    journalTraits,
                    out writer);

            return hasWriter;
        }

        public bool TryGetOrCreateWriter(
            JournalId journalId,
            out IJournalWriter writer)
        {
            var providerTraits =
                new[]
                {
                    new LocalStorageTrait(TriState.True)
                };

            return
                TryGetOrCreateWriter(
                    journalId,
                    providerTraits,
                    IJournalCurator.DefaultJournalTraits,
                    out writer);
        }

        public bool TryGetOrCreateWriter(
            JournalId journalId,
            Func<IEnumerable<IJournalWriterProvider>, IJournalWriterProvider> providerSelector,
            out IJournalWriter writer,
            IEnumerable<IJournalTrait> journalTraits = default)
        {
            writer = null;

            var provider =
                _writerProviders.FirstOrDefault(x => x.HasJournal(journalId)) ??
                providerSelector(_writerProviders);

            var hasWriter =
                true ==
                provider?.TryGetOrCreate(
                    journalId,
                    journalTraits ?? IJournalCurator.DefaultJournalTraits,
                    out writer);

            return hasWriter;
        }

        public bool TryGetOrCreateWriter(
            string connectionString,
            out IJournalWriter writer)
        {
            writer = null;

            var parsed = new JournalConnectionString(connectionString);

            var journalId =
                parsed.TryGetKey("JournalId", out var actualKey)
                    ? new JournalId(parsed[actualKey])
                    : default;

            var provider =
                _writerProviders.FirstOrDefault(x => journalId != default && x.HasJournal(journalId)) ??
                _writerProviders.FirstOrDefault(x => x.OwnsConnection(connectionString));

            var hasWriter =
                true ==
                provider?.TryGetOrCreate(
                    journalId,
                    IJournalCurator.DefaultJournalTraits,
                    out writer);

            return hasWriter;
        }
    }
}