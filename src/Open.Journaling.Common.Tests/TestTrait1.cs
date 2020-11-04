using System;
using Open.Journaling.Traits;

namespace Open.Journaling.Tests
{
    public class TestTrait1
        : IJournalTrait
    {
        public TestTrait1(
            TriState value)
        {
            Value = value;
        }

        public TriState Value { get; }
    }
}