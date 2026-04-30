using TaskFlow.UI.Authorization;
using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Data;

public class ActionCatalogSeeder
{
    private readonly AppActionRepository _repo;

    public ActionCatalogSeeder(AppActionRepository repo)
    {
        _repo = repo;
    }

    public async Task SeedAsync()
    {
        var existing = await _repo.GetAllAsync();

        foreach (var action in ActionCatalog.AppActions)
        {
            if (!existing.Any(a => a.Code == action.Code))
            {
                await _repo.CreateAsync(action);
            }
        }
    }
}