using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskFlow.UI.Models
{
    public class TaskItem
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ObjectId CreatedByUserId { get; set; }
        public ObjectId? AssignedToUserId { get; set; }

        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ObjectId WorkspaceId { get; set; }

        public string Priority { get; set; } = "Normal";
        public string Status { get; set; } = "Open";

        public bool IsRecurring { get; set; }
        public string RecurrenceType { get; set; } = string.Empty;
        public int? RecurrenceInterval { get; set; }

        public List<int> ReminderMinutesBefore { get; set; } = new();

        public List<TaskCustomField> CustomFields { get; set; } = new();

        [BsonIgnore]
        public string? AssignedToUserIdString
        {
            get => AssignedToUserId?.ToString();
            set => AssignedToUserId =
                string.IsNullOrWhiteSpace(value) ? null : ObjectId.Parse(value);
        }
        public DateTime? LastReminderSentUtc { get; set; }
        public DateTime? LastDeadlineAproachingNotifUtc { get; set; }
        public DateTime? LastDeadlineMissedNotifUtc { get; set; }
        public DateTime? LastRecurringCompletionNotifUtc { get; set; }
        
    }

    public class TaskCustomField
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}