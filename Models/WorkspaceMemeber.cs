using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskFlow.UI.Models
{
    [BsonIgnoreExtraElements]
    public class WorkspaceMember
    {
        public ObjectId UserId { get; set;  }
        public HashSet<string> RoleCodes { get; set; } = new();
        public HashSet<string> ActionCodes { get; set; } = new();
    }
}
