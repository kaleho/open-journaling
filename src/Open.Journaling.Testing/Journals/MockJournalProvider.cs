using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Moq;
using Open.Journaling.Traits;
using Open.Monikers;

namespace Open.Journaling.Journals
{
    public class MockJournalProvider
        : IJournalReaderProvider,
          IJournalWriterProvider
    {
        private delegate void CreateJournalReader(
            JournalId nameId,
            IEnumerable<IJournalTrait> traits,
            out IJournalReader journal);

        private delegate void CreateJournalWriter(
            JournalId nameId,
            IEnumerable<IJournalTrait> traits,
            out IJournalWriter journal);

        private readonly ConcurrentDictionary<JournalId, Lazy<IJournal>> _cache;
        public const string Name = "mockjournalprovider";
        public static ProviderId DefaultProviderId = new ProviderId(nameof(MockJournalProvider));

        public MockJournalProvider(
            ProviderId providerId = null,
            JournalTraits traits = null,
            IDictionary<JournalId, IJournal> journals = null)
        {
            Traits = traits ?? new JournalTraits(null);

            var journalList =
                journals != null
                    ? new Dictionary<JournalId, IJournal>(journals)
                    : new Dictionary<JournalId, IJournal>();

            _cache =
                new ConcurrentDictionary<JournalId, Lazy<IJournal>>(
                    journalList.ToDictionary(
                        x => x.Key,
                        x => new Lazy<IJournal>(() => x.Value)));

            ReaderProvider =
                GetReaderProvider(
                    providerId ?? DefaultProviderId,
                    Traits,
                    journalList);

            WriterProvider =
                GetWriterProvider(
                    providerId ?? DefaultProviderId,
                    Traits,
                    journalList);

            ConnectionString =
                string.Join(
                    ';',
                    "Name=",
                    Name);
        }

        public Mock<IJournalReaderProvider> ReaderProvider { get; }

        public Mock<IJournalWriterProvider> WriterProvider { get; }

        public ImmutableDictionary<JournalId, Lazy<IJournal>> Cache =>
            ImmutableDictionary.CreateRange(
                _cache.ToDictionary(
                    x => x.Key,
                    x => x.Value));

        public string ConnectionString { get; }

        public bool HasJournal(
            JournalId journalId)
        {
            return _cache.ContainsKey(journalId);
        }

        public bool HasTraits(
            IEnumerable<IJournalTrait> traits)
        {
            var returnValue = true;

            if (traits?.Any() == true)
            {
                foreach (var trait in traits)
                {
                    var currentTrait = Traits.Traits.FirstOrDefault(x => x.GetType() == trait.GetType());

                    if (currentTrait == null ||
                        trait.Value != TriState.Indeterminate &&
                        trait.Value != currentTrait.Value)
                    {
                        returnValue = false;

                        break;
                    }
                }
            }

            return returnValue;
        }

        public bool OwnsConnection(
            string connectionString)
        {
            var returnValue = false;

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var attributes =
                    connectionString
                        .Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.SplitToTuple("="))
                        .ToDictionary(
                            x => x.Item1.ToLowerInvariant(),
                            x => x.Item2.ToLowerInvariant());

                returnValue =
                    attributes.ContainsKey("providerid") && attributes["providerid"] == Name;
            }

            return returnValue;
        }

        public ProviderId ProviderId => new ProviderId("provider");

        public JournalTraits Traits { get; }

        public bool TryGetOrCreate(
            JournalId journalId,
            IEnumerable<IJournalTrait> traits,
            out IJournalReader reader)
        {
            if (_cache.ContainsKey(journalId))
            {
                reader = _cache[journalId].Value as IJournalReader;
            }
            else
            {
                if (ReaderProvider.Object.TryGetOrCreate(journalId, traits, out var existingReader))
                {
                    _cache.AddOrUpdate(
                        journalId,
                        typeId => new Lazy<IJournal>(() => existingReader),
                        (typeId, existing) => new Lazy<IJournal>(() => existingReader));
                }

                reader = existingReader;
            }

            return reader != default;
        }

        public bool TryGetOrCreate(
            JournalId journalId,
            IEnumerable<IJournalTrait> traits,
            out IJournalWriter writer)
        {
            if (_cache.ContainsKey(journalId))
            {
                writer = _cache[journalId].Value as IJournalWriter;
            }
            else
            {
                if (WriterProvider.Object.TryGetOrCreate(journalId, traits, out var existingWriter))
                {
                    _cache.AddOrUpdate(
                        journalId,
                        typeId => new Lazy<IJournal>(() => existingWriter),
                        (typeId, existing) => new Lazy<IJournal>(() => existingWriter));
                }

                writer = existingWriter;
            }

            return writer != default;
        }

        public static Mock<IJournalReaderProvider> GetReaderProvider(
            ProviderId providerId,
            JournalTraits traits,
            IDictionary<JournalId, IJournal> journals)
        {
            var returnValue = new Mock<IJournalReaderProvider>();

            returnValue
                .Setup(x => x.Cache)
                .Returns(
                    ImmutableDictionary.CreateRange(
                        journals.ToDictionary(
                            x => x.Key,
                            x => new Lazy<IJournal>(() => x.Value))));

            returnValue
                .Setup(x => x.ProviderId)
                .Returns(providerId);

            returnValue
                .Setup(x => x.Traits)
                .Returns(traits);

            returnValue
                .Setup(x => x.HasJournal(It.IsAny<JournalId>()))
                .Returns((JournalId typeId) => journals.ContainsKey(typeId));

            returnValue
                .Setup(x => x.HasTraits(It.IsAny<IEnumerable<IJournalTrait>>()))
                .Returns(
                    (IEnumerable<IJournalTrait> requiredTraits) =>
                    {
                        var hasTraits = true;

                        foreach (var trait in requiredTraits)
                        {
                            var currentTrait = traits.Traits.FirstOrDefault(x => x.GetType() == trait.GetType());

                            if (currentTrait == null ||
                                trait.Value != TriState.Indeterminate &&
                                trait.Value != currentTrait.Value)
                            {
                                hasTraits = false;

                                break;
                            }
                        }

                        return hasTraits;
                    });

            returnValue
                .Setup(
                    x =>
                        x.TryGetOrCreate(
                            It.IsAny<JournalId>(),
                            It.IsAny<IEnumerable<IJournalTrait>>(),
                            out It.Ref<IJournalReader>.IsAny))
                .Callback(
                    new CreateJournalReader(
                        (
                            JournalId journalId,
                            IEnumerable<IJournalTrait> traits,
                            out IJournalReader reader) =>
                        {
                            reader = journals.ContainsKey(journalId)
                                ? (IJournalReader) journals[journalId]
                                : new MockJournal(journalId);
                        }))
                .Returns(true);

            return returnValue;
        }

        public static Mock<IJournalWriterProvider> GetWriterProvider(
            ProviderId providerId,
            JournalTraits traits,
            IDictionary<JournalId, IJournal> journals)
        {
            var returnValue = new Mock<IJournalWriterProvider>();

            returnValue
                .Setup(x => x.Cache)
                .Returns(
                    ImmutableDictionary.CreateRange(
                        journals.ToDictionary(
                            x => x.Key,
                            x => new Lazy<IJournal>(() => x.Value))));

            returnValue
                .Setup(x => x.ProviderId)
                .Returns(providerId);

            returnValue
                .Setup(x => x.Traits)
                .Returns(traits);

            returnValue
                .Setup(x => x.HasJournal(It.IsAny<JournalId>()))
                .Returns((JournalId typeId) => journals.ContainsKey(typeId));

            returnValue
                .Setup(x => x.HasTraits(It.IsAny<IEnumerable<IJournalTrait>>()))
                .Returns(
                    (IEnumerable<IJournalTrait> requiredTraits) =>
                    {
                        var hasTraits = true;

                        foreach (var trait in requiredTraits)
                        {
                            var currentTrait = traits.Traits.FirstOrDefault(x => x.GetType() == trait.GetType());

                            if (currentTrait == null ||
                                trait.Value != TriState.Indeterminate &&
                                trait.Value != currentTrait.Value)
                            {
                                hasTraits = false;

                                break;
                            }
                        }

                        return hasTraits;
                    });

            returnValue
                .Setup(
                    x =>
                        x.TryGetOrCreate(
                            It.IsAny<JournalId>(),
                            It.IsAny<IEnumerable<IJournalTrait>>(),
                            out It.Ref<IJournalWriter>.IsAny))
                .Callback(
                    new CreateJournalWriter(
                        (
                            JournalId journalId,
                            IEnumerable<IJournalTrait> traits,
                            out IJournalWriter writer) =>
                        {
                            writer = journals.ContainsKey(journalId)
                                ? (IJournalWriter) journals[journalId]
                                : new MockJournal(journalId);
                        }))
                .Returns(true);

            return returnValue;
        }
    }
}