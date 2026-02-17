using Microsoft.AspNetCore.Authorization;

namespace TaskFlow.UI.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement> //This handler will run only for policies that include a PermissionRequirement.
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)// You never accidentally authorize guests
            {
                return Task.CompletedTask; 
            }

            var permissionClaims = context.User.FindAll("permission").Select(c => c.Value); 

            if (permissionClaims.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
