using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TaskFlow.UI.Models
{
    [BsonIgnoreExtraElements]
    public class Workspace
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<WorkspaceMember> Members { get; set; } = new();
        public ObjectId OwnerUserId { get; set; }

    }
}
