using MongoDB.Bson;
using TaskFlow.UI.Data;
using TaskFlow.UI.Models;
using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Authorization
{
    public class WorkspaceAuthorizationService
    {
        private readonly WorkspaceRoleRepository _roleRepo;
        private readonly WorkspaceBundleRepository _bundleRepo;

        public WorkspaceAuthorizationService(
            WorkspaceRoleRepository roleRepo,
            WorkspaceBundleRepository bundleRepo)
        {
            _roleRepo = roleRepo;
            _bundleRepo = bundleRepo;
        }

        private static string Norm(string code)
        {
            return code.Trim().ToLowerInvariant();
        }

        public async Task<HashSet<string>> GetEffectiveActionsAsync(
            Workspace workspace,
            ObjectId userId)
        {
            var member = workspace.Members.FirstOrDefault(m => m.UserId == userId);

            if (member == null)
                return new HashSet<string>();

            var actions = new HashSet<string>();

            actions.UnionWith(member.ActionCodes.Select(Norm));

            var roleCache = new Dictionary<string, WorkspaceRole?>();
            var bundleCache = new Dictionary<string, WorkspaceBundle?>();

            foreach (var roleCode in member.RoleCodes.Select(Norm))
            {
                if (!roleCache.TryGetValue(roleCode, out var role))
                {
                    role = await _roleRepo.GetByCodeAsync(roleCode);
                    roleCache[roleCode] = role;
                }

                if (role == null) continue;

                actions.UnionWith(role.Actions.Select(Norm));

                foreach (var bundleCode in role.Bundles.Select(Norm))
                {
                    if (!bundleCache.TryGetValue(bundleCode, out var bundle))
                    {
                        bundle = await _bundleRepo.GetByCodeAsync(bundleCode);
                        bundleCache[bundleCode] = bundle;
                    }

                    if (bundle == null) continue;

                    actions.UnionWith(bundle.Actions.Select(Norm));
                }
            }

            return actions;
        }

        public bool HasAction(HashSet<string> actions, string actionCode)
        {
            return actions.Contains(Norm(actionCode));
        }
    }
}