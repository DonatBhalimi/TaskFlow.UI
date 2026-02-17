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

            user.roles = new List<string> { role };

            var result = await _users.UpdateAsync(user);


        }



        public async Task RemoveRoleAsync(User user,string role)
        {
            if (user.roles.Contains(role))
            {
                user.roles.Remove(role);
                await _users.UpdateAsync(user);
            }
        }
    }
}
