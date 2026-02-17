using Microsoft.AspNetCore.Authorization;

namespace TaskFlow.UI.Authorization
{
    public class PermissionRequirement: IAuthorizationRequirement
    {
        public string Permission { get;}
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
