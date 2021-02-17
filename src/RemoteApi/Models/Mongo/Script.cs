using RemoteCommon.Connector.Mongo;

namespace RemoteApi.Models.Mongo
{
    public class Script : EntityWithTypedId<string>
    {
        public string Description { get; set; }
    }
}