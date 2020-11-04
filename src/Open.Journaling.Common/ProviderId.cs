using System.Runtime.Serialization;
using Open.Monikers;

namespace Open.Journaling
{
    [DataContract]
    public class ProviderId
        : RefId
    {
        public static RefId Mnt = new RefId("mnt");

        public ProviderId(
            string name, 
            RefId parentId = null)
            : base(name, parentId)
        {
        }

        public static ProviderId For(
            string name)
        {
            var returnValue =
                new ProviderId(
                    name,
                    new RefId(
                        "providers",
                        Mnt));

            return returnValue;
        }
    }
}