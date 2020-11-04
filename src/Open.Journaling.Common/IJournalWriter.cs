using System.Threading;
using System.Threading.Tasks;
using Open.Journaling.Model;

namespace Open.Journaling
{
    public interface IJournalWriter
        : IJournal
    {
        Task<IJournalEntry[]> Write(
            CancellationToken cancellationToken,
            params ISerializedEntry[] entries);
    }
}