using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Open.Journaling.Traits;

namespace Open.Journaling
{
    /// <summary>
    ///     The journal registry is to track all available journal
    ///     providers and supply readers, writers, and change feeds.
    /// </summary>
    public interface IJournalRegistry
    {
        //ImmutableList<IJournalWriterProvider> WriterProviders { get; }

        //ImmutableList<IJournalReaderProvider> ReaderProviders { get; }
    }

    public class JournalRegistry
    {
        private readonly ImmutableList<IJournalReaderProvider> _readerProviders;
        private readonly ImmutableList<IJournalWriterProvider> _writerProviders;

        public JournalRegistry(
            IEnumerable<IJournalReaderProvider> readerProviders,
            IEnumerable<IJournalWriterProvider> writerProviders)
        {
            _readerProviders =
                readerProviders == null
                    ? ImmutableList<IJournalReaderProvider>.Empty
                    : ImmutableList.CreateRange(readerProviders);

            _writerProviders =
                writerProviders == null
                    ? ImmutableList<IJournalWriterProvider>.Empty
                    : ImmutableList.CreateRange(writerProviders);
        }

        public bool HasJournalWriter(
            JournalId journalId)
        {
            return _writerProviders.Any(x => x.HasJournal(journalId));
        }

        public bool HasJournalReader(
            JournalId journalId)
        {
            return _readerProviders.Any(x => x.HasJournal(journalId));
        }

        public bool TryGetOrCreate(
            JournalId journalId,
            IEnumerable<IJournalTrait> traits,
            out IJournalWriter writer)
        {
            throw new NotImplementedException();
        }

        public bool TryGetOrCreate(
            JournalId journalId,
            out IJournalReader reader)
        {
            throw new NotImplementedException();
        }
    }
}