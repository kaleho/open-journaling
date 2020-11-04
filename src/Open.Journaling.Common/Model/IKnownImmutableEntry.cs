namespace Open.Journaling.Model
{
    /// <summary>
    /// An Entry where the manner in which the journal stores
    /// the entry is consistent with the EntryId and where
    /// the EntryId must be unique in the journal
    /// </summary>
    public interface IKnownImmutableEntry
        : ISerializedEntry
    {
        long Version { get; }
    }
}