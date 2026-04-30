using MongoDB.Driver;
using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Data
{
    public class ActionBundleRepository
    {
        private readonly IMongoCollection<ActionBundle> _bundles;
        public ActionBundleRepository(IMongoCollection<ActionBundle> bundle)
        {
            _bundles = bundle;
        }

        public Task CreateAsync(ActionBundle actionBundle)
        {
            return _bundles.InsertOneAsync(actionBundle);
        }
        public async Task<ActionBundle?> GetByCodeAsync(string code)
        {
            return await _bundles
                .Find(n =>  n.Code == code)
                .FirstOrDefaultAsync();
        }
    }
}
