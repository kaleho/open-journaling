using System;
using System.Threading;
using Open.Journaling.Journals;
using Open.Journaling.Model;
using Open.Journaling.Traits;
using Xunit;

namespace Open.Journaling.Testing.Tests
{
    public class MockJournalProviderTests
    {
        [Fact]
        public void Can_Create_Mock_JournalReader()
        {
            var provider = new MockJournalProvider();

            var typeId = new JournalId(Guid.NewGuid().ToString("N"));

            var result =
                provider.TryGetOrCreate(
                    typeId,
                    new IJournalTrait[0],
                    out IJournalReader journal);

            Assert.True(result);
            Assert.NotNull(journal);
            Assert.Equal(typeId.ToString(), journal.JournalId.ToString());
        }

        [Fact]
        public void Can_Create_Mock_JournalWriter()
        {
            var provider = new MockJournalProvider();

            var typeId = new JournalId(Guid.NewGuid().ToString("N"));

            var result =
                provider.TryGetOrCreate(
                    typeId,
                    new IJournalTrait[0],
                    out IJournalWriter journal);

            Assert.True(result);
            Assert.NotNull(journal);
            Assert.Equal(typeId.ToString(), journal.JournalId.ToString());
        }

        [Fact]
        public void TryGetOrCreate_Will_Return_Same_Journal_For_Reader_And_Writer()
        {
            var provider = new MockJournalProvider();

            var typeId = new JournalId(Guid.NewGuid().ToString("N"));

            var writerResult =
                provider.TryGetOrCreate(
                    typeId,
                    new IJournalTrait[0],
                    out IJournalWriter writer);

            var readerResult =
                provider.TryGetOrCreate(
                    typeId,
                    new IJournalTrait[0],
                    out IJournalReader reader);

            Assert.True(readerResult);
            Assert.True(writerResult);
            Assert.NotNull(reader);
            Assert.NotNull(writer);
            Assert.Single(provider.Cache);
            Assert.True(reader.Equals(writer));
        }

        [Fact]
        public void MockJournalWriter_Can_Read_And_Write_New_Entry()
        {
            var provider = new MockJournalProvider();

            var typeId = new JournalId(Guid.NewGuid().ToString("N"));

            var writerResult =
                provider.TryGetOrCreate(
                    typeId,
                    new IJournalTrait[0],
                    out IJournalWriter writer);

            var entry =
                new SerializedEntry(
                    "entry-1",
                    "",
                    "");

            writer.Write(
                CancellationToken.None,
                entry);

            var readerResult =
                provider.TryGetOrCreate(
                    typeId,
                    new IJournalTrait[0],
                    out IJournalReader reader);

            Assert.True(writerResult);
            Assert.NotNull(writer);
            Assert.Equal(typeId.ToString(), writer.JournalId.ToString());
            Assert.Single(((MockJournal)writer).Entries);
            Assert.Equal(writer.JournalId.ToString(), ((MockJournal)writer).Entries[0].JournalId);
            Assert.Equal(1, ((MockJournal)writer).Props.HighestSequenceNumber);
        }
    }
}