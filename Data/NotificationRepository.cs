using MongoDB.Bson;
using MongoDB.Driver;
using TaskFlow.UI.Models;

namespace TaskFlow.UI.Data
{
    public class NotificationRepository
    {
        private readonly IMongoCollection<Notification> _notification;

        public NotificationRepository(IMongoCollection<Notification> notification)
        {
            _notification = notification;
        }
        public Task CreateAsync(Notification notification)
        {
            return _notification.InsertOneAsync(notification);
        }

        public Task<List<Notification>> GetUnreadForUserAsync(ObjectId userId)
        {
            return _notification
                .Find(n => n.UserId == userId && !n.IsRead)
                .SortByDescending(n => n.CreatedAtUtc)
                .ToListAsync();
        }
        public Task MarkAsReadAsync(ObjectId notificationId)
        {
            var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
            return _notification.UpdateOneAsync(n => n.Id == notificationId, update);
        }
        public async Task<long> GetUnreadCountForUserAsync(ObjectId userId)
        {
            return await _notification.CountDocumentsAsync(n => n.UserId == userId && !n.IsRead);
        }

    }
}
