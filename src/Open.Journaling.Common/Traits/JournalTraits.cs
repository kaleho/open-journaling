using System.Collections.Generic;
using System.Collections.Immutable;

namespace Open.Journaling.Traits
{
    public class JournalTraits
    {
        public JournalTraits(
            IEnumerable<IJournalTrait> traits)
        {
            Traits =
                traits != null
                    ? ImmutableList.CreateRange(traits)
                    : ImmutableList<IJournalTrait>.Empty;
        }

        public ImmutableList<IJournalTrait> Traits { get; }
    }
}