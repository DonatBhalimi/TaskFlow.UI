using MongoDB.Driver;
using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Data
{
    public class WorkspaceBundleRepository
    {
        private readonly IMongoCollection<WorkspaceBundle> _bundles;
        public WorkspaceBundleRepository(IMongoCollection<WorkspaceBundle> bundles)
        {
            _bundles = bundles;
        }
        public Task <WorkspaceBundle?> GetByCodeAsync(string code)
        {
            return _bundles.Find(b =>  b.Code == code).FirstOrDefaultAsync();
        }
        public Task <List<WorkspaceBundle>> GetAll()
        {
            return _bundles.Find(_ => true).ToListAsync();
        }

    }
}
