using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CRM.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialCRM.Web.Models.User
{
    //Cutom Identity...
    public class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public string ConfirmationCode { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            return userIdentity;
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
    public class ApplicationUserRole : IdentityUserRole<int>
    {
        [Key, Column(Order = 0)]
        public override int RoleId { get; set; }

        //
        // Summary:
        //     UserId for the user that is in the role

        [Key, Column(Order = 1)]
        public override int UserId { get; set; }
    }

    public class ApplicationUserLogin : IdentityUserLogin<int>
    {
        [Key, Column(Order = 0)]
        public override int UserId { get; set; }
    }

    public class ApplicationUserClaim : IdentityUserClaim<int>
    {
        [Key, Column(Order = 0)]
        public override int Id { get; set; }
    }

    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>
    {
        //     [Key, Column(Order = 0)]
        //    public override int Id { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")//, throwIfV1Schema: false)
        {
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

}