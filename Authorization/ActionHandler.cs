using Microsoft.AspNetCore.Authorization;

namespace TaskFlow.UI.Authorization
{
    public class ActionHandler: AuthorizationHandler<ActionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActionRequirement requirement)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                return Task.CompletedTask;
            }
            var actions = context.User.FindAll("action").Select(c => c.Value);
            if (actions.Contains(requirement.ActionCode)) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
