
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
        private readonly ActionBundleRepository _bundleRepo;
        private readonly AppRoleRepository _roleRepo;
        public AuthService(UserRepository users, PasswordHasher<User> hasher, AppRoleRepository roleRepo,ActionBundleRepository bundleRepository)
        {
            _users = users;
            _hasher = hasher;
            _roleRepo = roleRepo;
            _bundleRepo = bundleRepository;
        }

        public async Task RegisterAsync(string email,string password)
        {
            email = email.Trim().ToLower();
            if (await _users.GetByEmail(email) != null)
            {
                throw new InvalidOperationException("User already exists");
            }
            var count = await _users.CountAsync();
            var user = new User
            {
                email = email,
                RoleCodes = count == 0 ? new HashSet<string> { "role.superchief"} : new HashSet<string>()
            };
            user.passwordHash = _hasher.HashPassword(user, password);
            await _users.CreateAsync(user);
            var saved = await _users.GetByEmail(email);
            
        }
        public static string Norm(string code)
        {
            return code.Trim().ToLowerInvariant(); 
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

            var roleCache = new Dictionary<string, AppRole>();
            var bundleCache = new Dictionary<string, ActionBundle>();
            var actionCodes = new HashSet<string>();

            actionCodes.UnionWith(user.ActionCodes.Select(Norm));

            foreach(var roleCode in user.RoleCodes.Select(Norm))
            {
                if (!roleCache.TryGetValue(roleCode,out var role))
                {
                    role = await _roleRepo.GetByCodeAsync(roleCode);
                    if (role !=null) roleCache[roleCode] = role;
                }
                if (role == null) continue;
                actionCodes.UnionWith(role.Actions.Select(Norm));

                foreach(var bundleCode in role.Bundles.Select(Norm))
                {
                    if(!bundleCache.TryGetValue(bundleCode,out var bundle))
                    {
                        bundle = await _bundleRepo.GetByCodeAsync(bundleCode);
                        if (bundle != null)
                        {
                            bundleCache[bundleCode] = bundle;
                        }
                    }
                    if (bundle == null) continue;
                    actionCodes.UnionWith(bundle.Actions.Select(Norm));
                }
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.email),
                new Claim("userId",user.Id.ToString())
            };
            foreach (var action in actionCodes) claims.Add(new Claim("action", Norm(action)));
            
            var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
             
            return new ClaimsPrincipal(identity);


        }
    }
}
