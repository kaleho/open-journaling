namespace Open.Journaling
{
    public interface IJournalProps
    {
        long HighestSequenceNumber { get; }

        long InitialUtcTicks { get; }
    }
}