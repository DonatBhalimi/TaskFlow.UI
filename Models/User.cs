using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskFlow.UI.Models
{
    public class User
    {
        [BsonIgnore]
        public string? selectedRole { get; set; }
        [BsonId]
        public ObjectId Id { get; set; }
        public string email { get; set; } = string.Empty;
        public string passwordHash { get; set; } = string.Empty;
        public List<string> roles { get; set; } = new();
        public List<string> permissionClaims { get; set; } = new();
    }
}
