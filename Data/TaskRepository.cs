using MongoDB.Bson;
using MongoDB.Driver;
using TaskFlow.UI.Models;
namespace TaskFlow.UI.Data
{
    public class TaskRepository
    {
        private readonly IMongoCollection<TaskItem> _tasks;
        public TaskRepository(IMongoCollection<TaskItem> tasks)
        {
            _tasks = tasks;
        }
        public async Task CreateAsync(TaskItem task)
        {
            await _tasks.InsertOneAsync(task);
        }
        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _tasks.Find(_ => true).ToListAsync();
        }
        public async Task UpdateAsync(TaskItem task)
        {
            await _tasks.ReplaceOneAsync(t => t.Id == task.Id, task);
        }

        public async Task<List<TaskItem>> GetForWorkspaceAsync(ObjectId userId, ObjectId workspaceId,bool canSeeAll)
        {
            
            if (canSeeAll)
            {
                return await _tasks.Find(t =>t.WorkspaceId==workspaceId).ToListAsync();
            }
            return await _tasks.Find(t =>
            t.WorkspaceId == workspaceId && (
            t.CreatedByUserId == userId ||
            t.AssignedToUserId == userId)).ToListAsync();
        }

    }
}
