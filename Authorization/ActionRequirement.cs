using Microsoft.AspNetCore.Authorization;

namespace TaskFlow.UI.Authorization
{
    public class ActionRequirement: IAuthorizationRequirement
    {
        public string ActionCode { get; set; }
        public ActionRequirement(string actionCode)
        {
            ActionCode = actionCode;    
        }

    }
}
