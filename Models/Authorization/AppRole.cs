using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskFlow.UI.Models.Authorization
{
    [BsonIgnoreExtraElements]
    public class AppRole
    {
        [BsonId]
        public ObjectId Id {  get; set; }
        public string Code { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public HashSet<string> Bundles { get; set; } = new();

        public HashSet<string> Actions { get; set; } = new();
    }
}
