using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SocialCRM.Web.Models.User;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(SocialCRM.Web.Startup))]
namespace SocialCRM.Web
{
    public partial class Startup
    {
        string baseUrl = ConfigurationManager.AppSettings["WebUrl"];
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }

        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            //var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
           // var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(context));
            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole,int,ApplicationUserRole>(context));
            var userManager = new ApplicationUserManager
                (
                new UserStore<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>(context)
                );
            userManager.EmailService = new EmailService();
            userManager.PasswordHasher = new ClearPassword();

            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin rool   
                var role = new ApplicationRole();
                role.Name = "Admin";
                roleManager.Create(role);

                // In Startup iam creating first Admin Role and creating a default Admin User 
                /*var Name = "Social CRM Admin";
                var UserName = ConfigurationManager.AppSettings["EmailUserName"];
                var Email = ConfigurationManager.AppSettings["EmailUserName"];
                var passwd = ConfigurationManager.AppSettings["EmailPassword"];

                //Here we create a Admin super user who will maintain the website                  
                var user = new ApplicationUser { ConfirmationCode = string.Empty, EmailConfirmed = true, UserName = UserName, Email = UserName, Name = Name };//, ProfileImage = model.ImageUrl };
                IdentityResult result = userManager.Create(user, passwd);
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                    var message = new IdentityMessage
                    {
                        Destination = Email,
                        Body = string.Format("Dear <b>{0}</b>,<br/><br/>You are assigned as Admin in 360 Insight CRM tool.<br/><br/>Use the following information to log in the system: <br/><br/>Website url:<b>{1}</b><br/>UserName: <b>{2}</b><br/>Password: <b>{3}</b><br/><br/>Once logged in the system, you are allowed to change your password.<br/><br/> Sincerely,<br/>360 Insight CRM Team", user.Name, baseUrl, UserName, passwd),
                        Subject = "360 Insight CRM- Invitation to Access"
                    };
                    userManager.EmailService.SendAsync(message);
                }*/
            }
        }
    }
}
