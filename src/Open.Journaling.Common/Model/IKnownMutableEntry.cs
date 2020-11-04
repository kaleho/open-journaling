namespace Open.Journaling.Model
{
    /// <summary>
    /// An Entry where the manner in which the journal stores
    /// the entry is consistent with the EntryId, suitable for
    /// an Upsert (Update or Insert) metaphor
    /// </summary>
    public interface IKnownMutableEntry
        : ISerializedEntry
    {
        long Version { get; }
    }
}