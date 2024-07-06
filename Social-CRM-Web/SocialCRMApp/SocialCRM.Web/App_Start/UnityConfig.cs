using Microsoft.Practices.Unity;
using SocialCRM.Web.Controllers.API;
using SocialCRM.Web.Repositories;
using SocialCRM.Web.Repositories.Core;
using System.Web.Http;
using Unity.WebApi;
using WebAPIDemo.Controllers;

namespace SocialCRM.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();            
            // register all your components with the container here
            container.RegisterType<ICompanyRepository, CompanyRepository>();
            container.RegisterType<IGroupRepository, GroupRepository>();
            container.RegisterType<ITeamRepository, TeamRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IProfileRepository, ProfileRepository>();
            container.RegisterType<IMiningRepository, MiningRepository>();
            container.RegisterType<ITaskRepository,TaskRepository>();
            container.RegisterType<ITwitterRepository, TwitterRepository>();
            container.RegisterType<IFacebookRepository, FacebookRepository>();
            container.RegisterType<IFeedlyRepository, FeedlyRepository>();
            container.RegisterType<INotificationsRepository, NotificationsRepository>();
            container.RegisterType<IPublisherRepository, PublisherRepository>();
            container.RegisterType<AccountController>(new InjectionConstructor(new UserRepository()));//Only for Account Controller.
            container.RegisterType<INotificationsRepository, NotificationsRepository>();
            container.RegisterType<IDashboardRepository, DashboardRepository>();
            container.RegisterType<IReportRepository, ReportRepository>();
            container.RegisterType<IAdminRepository, AdminRepository>();
            container.RegisterType<IUnifiedDeckRepository, UnifiedDeckRepository>();
            container.RegisterType<IOutlookRepository, OutlookRepository>();
            container.RegisterType<IBannedProfilesRepository, BannedProfilesRepository>();
            container.RegisterType<ICustomerProfileRepository, CustomerProfileRepository>();
            container.RegisterType<IGeneralSettingsRepository, GeneralSettingsRepository>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}