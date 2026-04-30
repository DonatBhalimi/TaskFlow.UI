using MongoDB.Bson;
using MongoDB.Driver;
using TaskFlow.UI.Authorization;
using TaskFlow.UI.Models;
using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Data
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;
        public UserRepository(IMongoCollection<User> users)
        {
            _users = users;
        }
        public Task<List<User>> GetAllAsync()
        {
            return _users.Find(_ => true).ToListAsync();
        }
        public Task<User?> GetByIdAsync(ObjectId id)
        {
            return _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }
        public Task<User?> GetByEmail(string email)
        {
            return _users.Find(u => u.email == email).FirstOrDefaultAsync();
        }
        public Task CreateAsync (User user)
        {
            return _users.InsertOneAsync(user); 
        }
        public async Task<ReplaceOneResult> UpdateAsync(User user)
        {
            return await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public Task<long> CountAsync()
        {
            return _users.CountDocumentsAsync(_ => true);
        }
        public async Task UpdateNotificationPreferencesAsync(User user)
        {
            var update = Builders<User>.Update
                .Set(u => u.InAppNotificationEnabled, user.InAppNotificationEnabled)
                .Set(u => u.TaskReminderNotificationEnabled, user.TaskReminderNotificationEnabled)
                .Set(u => u.DeadlineApproachingNotificationsEnabled, user.DeadlineApproachingNotificationsEnabled)
                .Set(u => u.DeadlineMissedNotificationEnabled, user.DeadlineMissedNotificationEnabled)
                .Set(u => u.RecurringCompletionNotificationEnabled, user.RecurringCompletionNotificationEnabled)
                .Set(u => u.DeadlineApproachingMinutesBefore, user.DeadlineApproachingMinutesBefore);
            await _users.UpdateOneAsync(u => u.Id == user.Id, update);
        }

    }
}
