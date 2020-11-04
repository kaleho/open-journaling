using System;

namespace Open.Journaling.Model
{
    public class LocationKind
    {
        public LocationKind(
            string kind)
        {
            Kind = kind;
        }

        public static LocationKind Sequence { get; } = new LocationKind("Sequence");

        public static LocationKind UtcTicks { get; } = new LocationKind("UtcTicks");

        public string Kind { get; }

        public static bool operator !=(
            LocationKind a,
            LocationKind b)
        {
            return !(a == b);
        }

        public static bool operator ==(
            LocationKind a,
            LocationKind b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public override bool Equals(
            object obj)
        {
            return
                ReferenceEquals(this, obj) ||
                obj is LocationKind other &&
                GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Kind);
        }

        public override string ToString()
        {
            return Kind;
        }
    }
}