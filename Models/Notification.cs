using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskFlow.UI.Models
{
    public class Notification
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public ObjectId? TaskId { get; set; }
        public ObjectId WorkspaceId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsRead {  get; set; } = false;
    }
}
