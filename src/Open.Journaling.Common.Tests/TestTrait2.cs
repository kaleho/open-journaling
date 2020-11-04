using System;
using Open.Journaling.Traits;

namespace Open.Journaling.Tests
{
    public class TestTrait2
        : IJournalTrait
    {
        public TestTrait2(
            TriState value)
        {
            Value = value;
        }

        public TriState Value { get; }
    }
}