using System;
using System.Linq;
using Xunit;

namespace Open.Journaling.Tests
{
    public class JournalConnectionStringTests
    {
        [Fact]
        public void Can_Set_Value_When_Exists()
        {
            var journalConnectionString =
                new JournalConnectionString("Key2=value2")
                {
                    ["key1"] = "value1"
                };

            Assert.Equal("value1", journalConnectionString["key1"]);
        }

        [Fact]
        public void Can_Set_Value_When_Not_Exist()
        {
            var journalConnectionString =
                new JournalConnectionString
                {
                    ["key1"] = "value1"
                };

            Assert.Equal("value1", journalConnectionString["key1"]);
        }

        [Fact]
        public void Will_Compare_Value_With_Same_And_Alternate_Casing()
        {
            var connectionString = "ProviderId=provider;JournalId=app;Url=https://localhost:5001";

            var parsed = new JournalConnectionString(connectionString);

            var hasSameCaseKeyNotValue = parsed.KeyHasValue("ProviderId", "Provider");
            var hasNotSameKeyAndSameCaseValue = parsed.KeyHasValue("providerId", "provider");

            Assert.True(hasSameCaseKeyNotValue);
            Assert.True(hasNotSameKeyAndSameCaseValue);
        }

        [Fact]
        public void Will_Get_Actual_Key_With_Same_And_Alternate_Casing()
        {
            var connectionString = "ProviderId=provider;JournalId=app;Url=https://localhost:5001";

            var parsed = new JournalConnectionString(connectionString);

            var hasSameCasedKey = parsed.TryGetKey("ProviderId", out var actualKey1);
            var hasAlternateCasedKey = parsed.TryGetKey("providerId", out var actualKey2);

            Assert.True(hasSameCasedKey);
            Assert.Equal("ProviderId", actualKey1);

            Assert.True(hasAlternateCasedKey);
            Assert.Equal("ProviderId", actualKey2);
        }

        [Fact]
        public void Will_Get_Default_Values_When_Keys_Do_Not_Exist()
        {
            var connectionString = "ProviderId=provider;JournalId=app;Url=https://localhost:5001";

            var journalConnectionString = new JournalConnectionString(connectionString);

            _ = journalConnectionString
                .TryCatch(
                    x => int.Parse(x["ReadInterval"]),
                    1_000,
                    out var readInterval);

            _ = journalConnectionString
                .TryCatch(
                    x => bool.Parse(x["UseUtcTicks"]),
                    true,
                    out var useUtcTicks);

            _ = journalConnectionString
                .TryCatch(
                    x => long.Parse(x["FromLocation"]),
                    10,
                    out var fromLocation);

            Assert.Equal(1_000, readInterval);
            Assert.True(useUtcTicks);
            Assert.Equal(10, fromLocation);
        }

        [Fact]
        public void Will_Get_Values_When_Keys_Do_Not_Exist()
        {
            var connectionString =
                "ProviderId=provider;JournalId=app;ReadInterval=1234;UseUtcTicks=True;FromLocation=9876";

            var journalConnectionString = new JournalConnectionString(connectionString);

            _ = journalConnectionString
                .TryCatch(
                    x => int.Parse(x["ReadInterval"]),
                    1_000,
                    out var readInterval);

            _ = journalConnectionString
                .TryCatch(
                    x => bool.Parse(x["UseUtcTicks"]),
                    false,
                    out var useUtcTicks);

            _ = journalConnectionString
                .TryCatch(
                    x => long.Parse(x["FromLocation"]),
                    10,
                    out var fromLocation);

            Assert.Equal(1234, readInterval);
            Assert.True(useUtcTicks);
            Assert.Equal(9876, fromLocation);
        }

        [Fact]
        public void Will_Parse_Correct_Number_Of_Attributes()
        {
            var connectionString = "ProviderId=provider;JournalId=app;Url=https://localhost:5001";

            var parsed = new JournalConnectionString(connectionString);

            Assert.Equal(3, parsed.Attributes.Count);

            Assert.Collection(
                parsed.Attributes.OrderBy(x => x.Key),
                pair =>
                {
                    Assert.Equal("JournalId", pair.Key);
                    Assert.Equal("app", pair.Value);
                },
                pair =>
                {
                    Assert.Equal("ProviderId", pair.Key);
                    Assert.Equal("provider", pair.Value);
                },
                pair =>
                {
                    Assert.Equal("Url", pair.Key);
                    Assert.Equal("https://localhost:5001", pair.Value);
                });
        }
    }
}