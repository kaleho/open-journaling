namespace Open.Journaling
{
    public interface IJournal
    {
        JournalId JournalId { get; }

        IJournalProps Props { get; }
    }
}