
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaskFlow.UI.Authorization;
using TaskFlow.UI.Data;

namespace TaskFlow.UI.Models.Authorization
{
    public class AuthService
    {
        private readonly UserRepository _users;
        private readonly PasswordHasher<User> _hasher;
        public AuthService(UserRepository users, PasswordHasher<User> hasher)
        {
            _users = users;
            _hasher = hasher;
        }

        public async Task RegisterAsync(string email,string password)
        {
            email = email.Trim().ToLower();
            if (await _users.GetByEmail(email) != null)
            {
                throw new InvalidOperationException("User already exists");
            }
            var existinguser = await _users.CountAsync();
            var user = new User
            {
                email = email,
                roles = existinguser == 0 ? new List<string> { "Owner" } : new List<string> { "Reader" }
            };
            user.passwordHash = _hasher.HashPassword(user, password);
            await _users.CreateAsync(user);
        }

        public async Task<ClaimsPrincipal> LoginAsync(string email,string password)
        {
            email = email.Trim().ToLower();
            var user = await _users.GetByEmail(email) ?? throw new InvalidOperationException("Invalid credentials");

            var results = _hasher.VerifyHashedPassword(user, user.passwordHash, password);

            if (results== PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Invalid credentials");
            }
            var claims = new List<Claim>
            {
                new Claim (ClaimTypes.Name,user.email)
            };
            var permissions = user.roles.SelectMany(RolePermissionMap.GetPermissionsRole).Distinct().ToList();

            claims.AddRange(
                permissions.Select(p => new Claim("permission", p)));
            claims.Add(new Claim("userId", user.Id.ToString()));
            
            var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
             
            return new ClaimsPrincipal(identity);


        }
    }
}
