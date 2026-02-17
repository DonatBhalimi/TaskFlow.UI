using Microsoft.AspNetCore.Identity;

namespace TaskFlow.UI.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public Guid OrganizationID { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }

}
