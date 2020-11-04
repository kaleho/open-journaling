using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Open.Journaling.Model;

namespace Open.Journaling.Journals
{
    public class MockJournal
        : IJournalReader,
            IJournalWriter
    {
        private readonly List<IJournalEntry> _entries;

        private long _highestSequenceNumber;

        public static JournalId DefaultMockJournalId = new JournalId(nameof(MockJournal));

        public MockJournal(
            JournalId journalId = null,
            IEnumerable<IJournalEntry> entries = null)
        {
            JournalId = journalId ?? DefaultMockJournalId;

            _entries = new List<IJournalEntry>(entries ?? new IJournalEntry[0]);

            _highestSequenceNumber =
                _entries.Any()
                    ? _entries.Max(e => e.Sequence)
                    : 0L;

            var props = new Mock<IJournalProps>();

            props
                .Setup(x => x.HighestSequenceNumber)
                .Returns(() => _highestSequenceNumber);

            props
                .Setup(x => x.InitialUtcTicks)
                .Returns(
                    _entries.Any()
                        ? _entries.Min(e => e.UtcTicks) - 1
                        : new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks);

            Props = props.Object;

            Reader = GetJournalReader(_entries);

            Writer = GetJournalWriter();
        }

        public MockJournal(
            Func<LocationKind, long, CancellationToken, long?, Task<IJournalEntry[]>> readFunc,
            Func<string, CancellationToken, Task<IJournalEntry>> readByEntryIdFunc,
            Func<LocationKind, long, CancellationToken, long?, string[], Task<IJournalEntry[]>> readWithTagsFunc,
            long? highestSequenceNumber = null,
            long? initialUtcTicks = null)
        {
            Reader = GetJournalReader(readFunc, readByEntryIdFunc, readWithTagsFunc);

            var props = new Mock<IJournalProps>();

            props
                .Setup(x => x.HighestSequenceNumber)
                .Returns(highestSequenceNumber ?? DefaultHighestSequenceNumber);

            props
                .Setup(x => x.InitialUtcTicks)
                .Returns(initialUtcTicks ?? DefaultInitialUtcTicks);

            Props = props.Object;
        }

        public static long DefaultHighestSequenceNumber { get; } = 0L;

        public static long DefaultInitialUtcTicks { get; } =
            new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc).Ticks;

        public ImmutableList<IJournalEntry> Entries => ImmutableList.CreateRange(_entries);

        public Mock<IJournalReader> Reader { get; }

        public Mock<IJournalWriter> Writer { get; }

        public JournalId JournalId { get; }

        public IJournalProps Props { get; }

        public Task<IJournalEntry[]> Read(
            LocationKind kind,
            long from,
            CancellationToken cancellationToken,
            long? to = null)
        {
            return Reader.Object.Read(kind, from, cancellationToken, to);
        }

        public Task<IJournalEntry> ReadByEntryId(
            string entryId,
            CancellationToken cancellationToken)
        {
            return Reader.Object.ReadByEntryId(entryId, cancellationToken);
        }

        public Task<IJournalEntry[]> ReadWithTags(
            LocationKind kind,
            long from,
            CancellationToken cancellationToken,
            long? to = null,
            params string[] tags)
        {
            return Reader.Object.ReadWithTags(kind, from, cancellationToken, to, tags);
        }

        public Task<IJournalEntry[]> Write(
            CancellationToken cancellationToken,
            params ISerializedEntry[] entries)
        {
            return Writer.Object.Write(cancellationToken, entries);
        }

        public static Mock<IJournalReader> GetJournalReader(
            Func<LocationKind, long, CancellationToken, long?, Task<IJournalEntry[]>> readFunc,
            Func<string, CancellationToken, Task<IJournalEntry>> readByEntryIdFunc,
            Func<LocationKind, long, CancellationToken, long?, string[], Task<IJournalEntry[]>> readWithTagsFunc)
        {
            var returnValue = new Mock<IJournalReader>();

            returnValue
                .Setup(
                    x =>
                        x.Read(
                            It.IsAny<LocationKind>(),
                            It.IsAny<long>(),
                            It.IsAny<CancellationToken>(),
                            It.IsAny<long?>()))
                .Returns(readFunc);

            returnValue
                .Setup(
                    x =>
                        x.ReadByEntryId(
                            It.IsAny<string>(),
                            It.IsAny<CancellationToken>()))
                .Returns(readByEntryIdFunc);

            returnValue
                .Setup(
                    x =>
                        x.ReadWithTags(
                            It.IsAny<LocationKind>(),
                            It.IsAny<long>(),
                            It.IsAny<CancellationToken>(),
                            It.IsAny<long>(),
                            It.IsAny<string[]>()))
                .Returns(readWithTagsFunc);

            return returnValue;
        }

        public static Mock<IJournalReader> GetJournalReader(
            IEnumerable<IJournalEntry> entries)
        {
            return
                GetJournalReader(
                    (kind, from, token, to) =>
                        Task.FromResult(
                            (kind == LocationKind.Sequence
                                ? entries.Where(
                                    x =>
                                        x.Sequence > from &&
                                        x.Sequence <= to)
                                : entries.Where(
                                    x =>
                                        x.UtcTicks > from &&
                                        x.UtcTicks <= to))
                            .ToArray()),
                    (entryId, token) =>
                        Task.FromResult(
                            entries.FirstOrDefault(
                                x => x.EntryId == entryId)),
                    (kind, from, token, to, tags) =>
                        Task.FromResult(
                            (kind == LocationKind.Sequence
                                ? entries.Where(
                                    x =>
                                        x.Sequence > from &&
                                        x.Sequence <= to &&
                                        x.Tags.Any(tags.Contains))
                                : entries.Where(
                                    x =>
                                        x.UtcTicks > from &&
                                        x.UtcTicks <= to &&
                                        x.Tags.Any(tags.Contains)))
                            .ToArray()));
        }

        public Mock<IJournalWriter> GetJournalWriter(
            Func<CancellationToken, ISerializedEntry[], Task<IJournalEntry[]>> writeFunc)
        {
            var returnValue = new Mock<IJournalWriter>();

            returnValue
                .Setup(
                    x =>
                        x.Write(
                            It.IsAny<CancellationToken>(),
                            It.IsAny<ISerializedEntry[]>()))
                .Returns(writeFunc);

            return returnValue;
        }

        public Mock<IJournalWriter> GetJournalWriter()
        {
            return
                GetJournalWriter(
                    (token, newEntries) =>
                        Task.FromResult(
                            WriteJournalEntries(newEntries)));
        }

        private static void SetValue(
            Type fromType,
            object fromObject,
            Type toType,
            object toObject,
            string propertyName)
        {
            SetValue(
                toType,
                toObject,
                propertyName,
                fromType
                    .GetProperty(propertyName)
                    ?.GetValue(fromObject));
        }

        private static void SetValue(
            Type toType,
            object toObject,
            string propertyName,
            object value)
        {
            var propertyInfo =
                toType.GetProperty(
                    propertyName,
                    BindingFlags.SetProperty |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance);

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(toObject, value);
            }

            var runtimeFields = toType.GetRuntimeFields();

            var runtimeSetter = runtimeFields.FirstOrDefault(x => x.Name.Contains($"<{propertyName}>"));

            if (runtimeSetter != null)
            {
                runtimeSetter.SetValue(toObject, value);
            }
        }

        private IJournalEntry[] WriteJournalEntries(
            ISerializedEntry[] newEntries)
        {
            var list = new List<IJournalEntry>();

            var journalEntryType = typeof(JournalEntry);

            var serializedType = typeof(ISerializedEntry);

            foreach (var serializedEntry in newEntries)
            {
                var newJournalEntry = (IJournalEntry)Activator.CreateInstance(typeof(JournalEntry), true);

                SetValue(serializedType, serializedEntry, journalEntryType, newJournalEntry, nameof(IJournalEntry.EntryId));

                SetValue(serializedType, serializedEntry, journalEntryType, newJournalEntry, nameof(IJournalEntry.Tags));

                SetValue(serializedType, serializedEntry, journalEntryType, newJournalEntry, "Meta");
                SetValue(serializedType, serializedEntry, journalEntryType, newJournalEntry, "Payload");

                SetValue(journalEntryType, newJournalEntry, nameof(IJournalEntry.JournalId), JournalId.ToString());
                SetValue(journalEntryType, newJournalEntry, nameof(IJournalEntry.Sequence), ++_highestSequenceNumber);
                SetValue(journalEntryType, newJournalEntry, nameof(IJournalEntry.UtcTicks), DateTime.UtcNow.Ticks);

                list.Add(newJournalEntry);
            }

            _entries.AddRange(list);

            return list.ToArray();
        }
    }
}