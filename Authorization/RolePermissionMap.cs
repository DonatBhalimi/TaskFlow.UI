using TaskFlow.UI.Models.Authorization;

namespace TaskFlow.UI.Authorization
{
    public class RolePermissionMap
    {
        public static List<string> GetPermissionsRole(string role ) 
        {
            return role switch
            {
                "Owner" => new List<string>
            {
                Permissions.ManageSystem,
                Permissions.ManageOrganization,
                Permissions.ManageUsers,
                Permissions.ManageWorkspaces,
                Permissions.ManageTemplates,
                Permissions.ManageSettings,
                Permissions.CreateTasks,
                Permissions.AssignTasks,
                Permissions.EditTasks,
                Permissions.ViewTasks
            },
                "OrgOwner" => new List<string>
            {
                Permissions.ManageUsers,
                Permissions.ManageOrganization,
                Permissions.ManageUsers,
                Permissions.ManageWorkspaces,
                Permissions.ManageTemplates,
                Permissions.ManageSettings,
                Permissions.CreateTasks,
                Permissions.AssignTasks,
                Permissions.EditTasks,
                Permissions.ViewTasks
            },
                "Manager" => new List<string>
            {
                Permissions.ManageWorkspaces,
                Permissions.CreateTasks,
                Permissions.AssignTasks,
                Permissions.EditTasks,
                Permissions.ViewTasks
            },
                "Creator" => new List<string>
            {
                Permissions.CreateTasks,
                Permissions.EditTasks,
                Permissions.ViewTasks
            },
                "Reader" => new List<string>
            {
                Permissions.ViewTasks
            },
                _ => new List<string>()
            };
            
        }
    }
}
