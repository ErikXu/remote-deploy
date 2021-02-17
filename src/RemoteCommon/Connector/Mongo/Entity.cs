using MongoDB.Bson;

namespace RemoteCommon.Connector.Mongo
{
    public abstract class EntityWithTypedId<TId>
    {
        public TId Id { get; set; }
    }

    public abstract class Entity : EntityWithTypedId<ObjectId>
    {

    }
}