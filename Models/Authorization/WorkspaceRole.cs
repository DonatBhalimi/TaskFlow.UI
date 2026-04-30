using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskFlow.UI.Models.Authorization
{
    [BsonIgnoreExtraElements]
    public class WorkspaceRole
    {
        [BsonId] 
        public ObjectId Id { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public HashSet<string> Bundles { get; set; } = new();
        public HashSet<string> Actions { get; set; }= new();

    }
}
