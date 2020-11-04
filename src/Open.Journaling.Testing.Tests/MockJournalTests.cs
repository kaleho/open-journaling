using System;
using System.Threading;
using Open.Journaling.Journals;
using Open.Journaling.Model;
using Xunit;

namespace Open.Journaling.Testing.Tests
{
    public class MockJournalTests
    {
        [Fact]
        public void Can_Write_Items_1x()
        {
            var journal = new MockJournal();

            var entry = new SerializedEntry(
                "entry-1",
                "",
                "");

            journal.Write(
                CancellationToken.None,
                entry);

            Assert.Single(journal.Entries);
            Assert.Equal(1, journal.Props.HighestSequenceNumber);
        }

        [Fact]
        public void Can_Write_Items_10x()
        {
            var journal = new MockJournal();

            var entries =
                10.Items(
                    i =>
                        new SerializedEntry(
                            $"entry-{i + 1}",
                            "",
                            ""));

            journal.Write(
                CancellationToken.None,
                entries.ToArray());

            Assert.Equal(10, journal.Entries.Count);
            Assert.Equal(10, journal.Props.HighestSequenceNumber);
        }
    }
}
