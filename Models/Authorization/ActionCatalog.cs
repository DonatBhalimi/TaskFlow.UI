
using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Authorization;

public static class ActionCatalog
{
    public static readonly AppAction[] AppActions =
    {
        

        new()
        {
            Code = "app.users.manage",
            Scope = ActionScopes.App,
            DisplayName = "Manage Users",
            Description = "Create, update and delete users."
        },
        new()
        {
            Code = "app.roles.manage",
            Scope = ActionScopes.App,
            DisplayName = "Manage Roles",
            Description = "Create and modify application roles."
        },
        new()
        {
            Code = "app.actions.manage",
            Scope = ActionScopes.App,
            DisplayName = "Manage Actions",
            Description = "Create and modify action definitions."
        },
        new()
        {
            Code = "app.settings.manage",
            Scope = ActionScopes.App,
            DisplayName = "Manage Settings",
            Description = "Modify system-wide settings."
        },
        new()
        {
            Code = "workspaces.create",
            Scope = ActionScopes.App,
            DisplayName = "Create Workspace",
            Description = "Create new workspaces."
        },


        new()
        {
            Code = "ws.tasks.view",
            Scope = ActionScopes.Workspace,
            DisplayName = "View Tasks",
            Description = "View tasks within workspace."
        },
        new()
        {
            Code = "ws.tasks.viewAll",
            Scope = ActionScopes.Workspace,
            DisplayName = "View All Tasks",
            Description = "View all tasks regardless of assignment."
        },
        new()
        {
            Code = "ws.tasks.create",
            Scope = ActionScopes.Workspace,
            DisplayName = "Create Tasks",
            Description = "Create new tasks."
        },
        new()
        {
            Code = "ws.tasks.edit",
            Scope = ActionScopes.Workspace,
            DisplayName = "Edit Tasks",
            Description = "Edit any task."
        },
        new()
        {
            Code = "ws.tasks.editOwn",
            Scope = ActionScopes.Workspace,
            DisplayName = "Edit Own Tasks",
            Description = "Edit tasks created by the user."
        },
        new()
        {
            Code = "ws.tasks.assign",
            Scope = ActionScopes.Workspace,
            DisplayName = "Assign Tasks",
            Description = "Assign tasks to workspace members."
        },
        new()
        {
            Code = "ws.members.invite",
            Scope = ActionScopes.Workspace,
            DisplayName = "Invite Members",
            Description = "Invite users into workspace."
        },
        new()
        {
            Code = "ws.members.remove",
            Scope = ActionScopes.Workspace,
            DisplayName = "Remove Members",
            Description = "Remove users from workspace."
        },
        new()
        {
            Code = "ws.members.permissions.edit",
            Scope = ActionScopes.Workspace,
            DisplayName = "Edit Member Permissions",
            Description = "Modify workspace permissions of members."
        }
    };
}