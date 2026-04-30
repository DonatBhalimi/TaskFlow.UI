using MongoDB.Bson;
using MongoDB.Driver;
using TaskFlow.UI.Authorization;
using TaskFlow.UI.Models;

namespace TaskFlow.UI.Data
{
    public class WorkspaceRepository
    {
        private readonly IMongoCollection<Workspace> _workspaces;
        public WorkspaceRepository(IMongoCollection<Workspace> workspaces)
        {
            _workspaces = workspaces;
        }
        public async Task CreateAsync(Workspace workspace)
        {
            await _workspaces.InsertOneAsync(workspace);
        }
        public async Task<List<Workspace>> GetForUserId(ObjectId userId)
        {
            return await _workspaces.Find(w => w.Members.Any(m=> m.UserId == userId)).ToListAsync();
        }
        public async Task<Workspace?> GetByIdAsync(ObjectId id)
        {
            return await _workspaces.Find(w => w.Id == id).FirstOrDefaultAsync();
        }
        public async Task AddMemberAsync(ObjectId workspaceId, ObjectId userId,string roleCode)
        {
            var member = new WorkspaceMember
            {
                UserId = userId,
                RoleCodes= new HashSet<string> { roleCode.Trim().ToLowerInvariant()},
                ActionCodes = new HashSet<string>()
            };
            var update = Builders<Workspace>.Update.AddToSet(
                w => w.Members, member);
             
            await _workspaces.UpdateOneAsync(
                w => w.Id == workspaceId, update);
        }
        public async Task RemoveMemberAsync(ObjectId workspaceId, ObjectId userId)
        {
            var update = Builders<Workspace>.Update.PullFilter(
                w => w.Members, m => m.UserId == userId);

            await _workspaces.UpdateOneAsync(
                w => w.Id == workspaceId, update);
        }

        public async Task UpdateMemberFeatureAsync(ObjectId workspaceId, ObjectId userId, string feature, bool enabled)
        {
            var filter = Builders<Workspace>.Filter.And(
                Builders<Workspace>.Filter.Eq(w => w.Id, workspaceId),
                Builders<Workspace>.Filter.ElemMatch(w => w.Members, m => m.UserId == userId));

            UpdateDefinition<Workspace> update;

            if (enabled)
            {
                update = Builders<Workspace>.Update.AddToSet("Members.$.ActionCodes", feature);
            }
            else
            {
                update = Builders<Workspace>.Update.Pull("Members.$.ActionCodes", feature);
            }

            await _workspaces.UpdateOneAsync(filter, update);
        }
    }
}
