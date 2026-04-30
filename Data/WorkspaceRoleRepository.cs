using MongoDB.Driver;
using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Data
{
    public class WorkspaceRoleRepository
    {
        private readonly IMongoCollection<WorkspaceRole> _roles;

        public WorkspaceRoleRepository(IMongoCollection<WorkspaceRole> roles)
        {
            _roles = roles;
        }
        public Task<WorkspaceRole?> GetByCodeAsync(string code)
        {
            return _roles.Find(r => r.Code == code).FirstOrDefaultAsync();
        } 
        public Task <List<WorkspaceRole>> GetAllAsync()
        {
            return _roles.Find(_ => true).ToListAsync();
        }
    }
}
