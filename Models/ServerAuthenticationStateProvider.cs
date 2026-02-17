using Microsoft.AspNetCore.Components.Authorization;

namespace TaskFlow.UI.Models
{
    public class ServerAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _http;
        public ServerAuthenticationStateProvider (IHttpContextAccessor http)
        {
            _http = http;
        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = _http.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
            return Task.FromResult(new AuthenticationState(user));
        }
    }
}
