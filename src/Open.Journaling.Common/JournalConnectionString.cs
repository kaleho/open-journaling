using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Open.Journaling.Traits;
using Open.Monikers;

namespace Open.Journaling
{
    public class JournalConnectionString
    {
        public JournalConnectionString()
        {
            Attributes = ImmutableDictionary<string, string>.Empty;
        }

        public JournalConnectionString(
            IDictionary<string, string> attributes)
        {
            Attributes =
                ImmutableDictionary<string, string>
                    .Empty
                    .AddRange(attributes);
        }

        public JournalConnectionString(
            string connectionString)
            : this(
                string.IsNullOrWhiteSpace(connectionString)
                    ? throw new ArgumentException($"Argument is missing or invalid: {nameof(connectionString)}")
                    : connectionString
                      .Split(';', StringSplitOptions.RemoveEmptyEntries)
                      .Select(x => x.SplitToTuple("="))
                      .ToDictionary(
                          x => x.Item1,
                          x => x.Item2))
        {
        }

        public string this[
            string key,
            bool ignoreCase = true]
        {
            get =>
                TryGetValue(key, out var value, ignoreCase)
                    ? value
                    : throw new Exception($"Key ({key}) was not found in connection string.");

            set =>
                Attributes =
                    Attributes.SetItem(
                        TryGetKey(key, out var actualKey) ? actualKey : key,
                        value);
        }

        public ImmutableDictionary<string, string> Attributes { get; private set; }

        public bool KeyHasValue(
            string key,
            string value,
            bool ignoreKeyCasing = true,
            bool ignoreValueCasing = true)
        {
            var returnValue =
                TryGetKey(key, out var actualKey, ignoreKeyCasing) &&
                Attributes[actualKey].Equals(
                    value,
                    ignoreValueCasing
                        ? StringComparison.OrdinalIgnoreCase
                        : StringComparison.Ordinal);

            return returnValue;
        }

        public bool TryGetKey(
            string key,
            out string actualKey,
            bool ignoreCase = true)
        {
            actualKey =
                Attributes.Keys.FirstOrDefault(
                    x =>
                        x == key ||
                        ignoreCase &&
                        x.ToLowerInvariant().Equals(key.ToLowerInvariant()));

            return actualKey != default;
        }

        public bool TryGetValue(
            string key,
            out string value,
            bool ignoreCase = true)
        {
            var returnValue = false;

            if (TryGetKey(key, out var actualKey))
            {
                value = Attributes[actualKey];

                returnValue = true;
            }
            else
            {
                value = default;
            }

            return returnValue;
        }
    }
}