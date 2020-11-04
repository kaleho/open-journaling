using System.Threading;
using System.Threading.Tasks;
using Open.Journaling.Model;

namespace Open.Journaling
{
    public interface IJournalReader
        : IJournal
    {
        Task<IJournalEntry[]> Read(
            LocationKind kind,
            long from,
            CancellationToken cancellationToken,
            long? to = null);

        Task<IJournalEntry> ReadByEntryId(
            string entryId,
            CancellationToken cancellationToken);

        Task<IJournalEntry[]> ReadWithTags(
            LocationKind kind,
            long from,
            CancellationToken cancellationToken,
            long? to = null,
            params string[] tags);
    }
}