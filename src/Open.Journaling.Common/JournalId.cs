using Open.Monikers;

namespace Open.Journaling
{
    public class JournalId
        : RefId
    {
        public JournalId(
            string name,
            RefId parentId = null)
            : base(name, parentId)
        {
        }

        public static JournalId For(
            RefId parentId)
        {
            return new JournalId("journal", parentId);
        }
    }
}