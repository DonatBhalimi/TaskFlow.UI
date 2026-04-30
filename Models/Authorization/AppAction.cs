using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskFlow.UI.Models.Authorization
{
    [BsonIgnoreExtraElements]
    public class AppAction
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DisplayName {  get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
    }
}
