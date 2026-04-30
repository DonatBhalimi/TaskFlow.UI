using MongoDB.Driver;
using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Data
{
    public class WorkspaceAuthSeeder
    {
        private readonly IMongoCollection<WorkspaceAction> _actions;
        private readonly IMongoCollection<WorkspaceBundle> _bundles;
        private readonly IMongoCollection<WorkspaceRole> _roles;

        public WorkspaceAuthSeeder(IMongoCollection<WorkspaceAction> actions, IMongoCollection<WorkspaceBundle> bundles, IMongoCollection<WorkspaceRole> roles)
        {
            _actions = actions;
            _bundles = bundles;
            _roles = roles;
        }

        public async Task SeedAsync()
        {
            await SeedActionsAsync();
            await SeedBundlesAsync();
            await SeedRolesAsync();
        }
        private async Task SeedActionsAsync()
        {
            var actions = new List<WorkspaceAction>
            {
                new() { Code = "ws.tasks.view", DisplayName = "View Tasks", Description = "Can view assigned tasks" },
                new() { Code = "ws.tasks.view-all", DisplayName = "View All Tasks", Description = "Can view all workspace tasks" },
                new() { Code = "ws.tasks.create", DisplayName = "Create Tasks", Description = "Can create tasks" },
                new() { Code = "ws.tasks.edit", DisplayName = "Edit Tasks", Description = "Can edit any workspace task" },
                new() { Code = "ws.tasks.edit-own", DisplayName = "Edit Own Tasks", Description = "Can edit own tasks" },
                new() { Code = "ws.tasks.assign", DisplayName = "Assign Tasks", Description = "Can assign tasks to others" },
                new() { Code = "ws.members.invite", DisplayName = "Invite Members", Description = "Can invite members to workspace" },
                new() { Code = "ws.members.remove", DisplayName = "Remove Members", Description = "Can remove members from workspace" },
                new() { Code = "ws.members.change-role", DisplayName = "Change Member Roles", Description = "Can change workspace roles" }
            };

            foreach (var action in actions)
            {
                var exists = await _actions.Find(a => a.Code == action.Code).AnyAsync();
                if (!exists)
                    await _actions.InsertOneAsync(action);
            }
        }

        private async Task SeedBundlesAsync()
        {
            var bundles = new List<WorkspaceBundle>
            {
                new()
                {
                    Code = "ws.bundle.reader",
                    DisplayName = "Reader Bundle",
                    Description = "Basic read access",
                    Actions = new HashSet<string>
                    {
                        "ws.tasks.view"
                    }
                },
                new()
                {
                    Code = "ws.bundle.creator",
                    DisplayName = "Creator Bundle",
                    Description = "Task creation and own task editing",
                    Actions = new HashSet<string>
                    {
                        "ws.tasks.view",
                        "ws.tasks.view-all",
                        "ws.tasks.create",
                        "ws.tasks.edit-own"
                    }
                },
                new()
                {
                    Code = "ws.bundle.manager",
                    DisplayName = "Manager Bundle",
                    Description = "Task management bundle",
                    Actions = new HashSet<string>
                    {
                        "ws.tasks.view",
                        "ws.tasks.view-all",
                        "ws.tasks.create",
                        "ws.tasks.edit",
                        "ws.tasks.edit-own",
                        "ws.tasks.assign"
                    }
                },
                new()
                {
                    Code = "ws.bundle.member-admin",
                    DisplayName = "Member Admin Bundle",
                    Description = "Workspace member administration",
                    Actions = new HashSet<string>
                    {
                        "ws.members.invite",
                        "ws.members.remove",
                        "ws.members.change-role"
                    }
                }
            };

            foreach (var bundle in bundles)
            {
                var exists = await _bundles.Find(b => b.Code == bundle.Code).AnyAsync();
                if (!exists)
                    await _bundles.InsertOneAsync(bundle);
            }
        }

        private async Task SeedRolesAsync()
        {
            var roles = new List<WorkspaceRole>
            {
                new()
                {
                    Code = "ws.role.reader",
                    DisplayName = "Reader",
                    Description = "Read-only workspace member",
                    Bundles = new HashSet<string>
                    {
                        "ws.bundle.reader"
                    }
                },
                new()
                {
                    Code = "ws.role.creator",
                    DisplayName = "Creator",
                    Description = "Can create tasks and edit own tasks",
                    Bundles = new HashSet<string>
                    {
                        "ws.bundle.creator"
                    }
                },
                new()
                {
                    Code = "ws.role.manager",
                    DisplayName = "Manager",
                    Description = "Can manage and assign tasks",
                    Bundles = new HashSet<string>
                    {
                        "ws.bundle.manager"
                    }
                },
                new()
                {
                    Code = "ws.role.owner",
                    DisplayName = "Owner",
                    Description = "Full workspace control",
                    Bundles = new HashSet<string>
                    {
                        "ws.bundle.manager",
                        "ws.bundle.member-admin"
                    }
                }
            };

            foreach (var role in roles)
            {
                var exists = await _roles.Find(r => r.Code == role.Code).AnyAsync();
                if (!exists)
                    await _roles.InsertOneAsync(role);
            }
        }
    }
}
