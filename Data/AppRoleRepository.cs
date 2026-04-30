using MongoDB.Driver;
using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Data
{
    public class AppRoleRepository
    {
        private readonly IMongoCollection<AppRole> _roles;
        public AppRoleRepository(IMongoCollection<AppRole> roles)
        {
            _roles = roles; 
        }
        public Task<List<AppRole>> GetAllAsync()
        {
            return _roles.Find(_ => true).ToListAsync();
        }

        public Task<AppRole?> GetByCodeAsync(string code)
        {
            return _roles.Find(r => r.Code == code).FirstOrDefaultAsync();
        }
        public Task CreateAsync(AppRole role)
        {
            return _roles.InsertOneAsync(role);
        }
    }
}
