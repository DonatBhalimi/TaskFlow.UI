using MongoDB.Driver;
using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Data
{
    public class AppActionRepository
    {
        private readonly IMongoCollection<AppAction> _actions;
        public AppActionRepository(IMongoCollection<AppAction> actions)
        {
            _actions = actions;
        }
        public Task<List<AppAction>> GetAllAsync() => _actions.Find(_ => true).ToListAsync();

        public Task<AppAction?> GetByCodeAsync(string code) => _actions.Find(a => a.Code == code).FirstOrDefaultAsync();

        public Task CreateAsync(AppAction action) => _actions.InsertOneAsync(action);
    }
}
