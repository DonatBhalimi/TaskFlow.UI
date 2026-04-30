using TaskFlow.UI.Data;

namespace TaskFlow.UI.Models.Authorization
{
    public class UserRoleService
    {
        private readonly UserRepository _users;
        private readonly ILogger<UserRoleService> _logger;

        public UserRoleService(UserRepository users, ILogger<UserRoleService> logger)
        {
            _users = users;
            _logger = logger;
        }

        public async Task AssignRoleAsync(User user, string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return;

            var normalized = role.Trim().ToLowerInvariant();

            user.RoleCodes.Add(normalized);

            await _users.UpdateAsync(user);


        }



        public async Task RemoveRoleAsync(User user,string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return;

            var normalized = role.Trim().ToLowerInvariant();

            if (user.RoleCodes.Remove(normalized))
            {
                await _users.UpdateAsync(user);
            }
        }
    }
}
