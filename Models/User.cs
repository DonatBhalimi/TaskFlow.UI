using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskFlow.UI.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonIgnore]
        public string? selectedRole { get; set; }

        [BsonId]
        public ObjectId Id { get; set; }

        public string email { get; set; } = string.Empty;
        public string passwordHash { get; set; } = string.Empty;

        public HashSet<string> RoleCodes { get; set; } = new();
        public HashSet<string> ActionCodes { get; set; } = new();

        [BsonIgnore]
        public string? selected { get; set; }

        public bool InAppNotificationEnabled { get; set; } = true;
        public bool TaskReminderNotificationEnabled { get; set; } = true;
        public bool DeadlineApproachingNotificationsEnabled { get; set; } = true;
        public bool DeadlineMissedNotificationEnabled { get; set; } = true;
        public bool RecurringCompletionNotificationEnabled { get; set; } = true;
        public int DeadlineApproachingMinutesBefore { get; set; } = 60;

    }
}