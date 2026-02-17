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
        
    }
}
