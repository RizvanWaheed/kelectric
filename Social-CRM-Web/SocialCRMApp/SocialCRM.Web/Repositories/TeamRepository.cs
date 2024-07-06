using CRM.Common;
using CRM.Common.Enum;
using CRM.Entities;
using SocialCRM.Web.Models;
using SocialCRM.Web.Models.Team;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        public async Task<TeamViewModel> GetByCompanyId(int companyId)
        {
            string Newdate =DateTime.Now.ToString();
            var result = new TeamViewModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        result = db.UserCompanies.Where(x => x.CompanyId == companyId).Select(y => new TeamViewModel
                        {
                            TotalGroup = y.Company.Groups.Count(),
                            TotalMember = y.Company.Teams.Count,
                            TotalProfile = y.Company.ProfileComapnyGroups.Distinct().Count(),
                            CompanyName = y.Company.CompanyName,
                            Team = y.Company.Teams.Select(z => new TeamMember
                            {
                                Email = z.AspNetUser.Email,
                                Name = z.FirstName + " " + z.LastName,
                                ProfileUrl = z.AspNetUser.ProfileImage == null ? null : z.AspNetUser.ProfileImage + "?" + Newdate.ToString(),
                                UserId = z.UserId ?? 0,
                                TeamMemberId = z.Id,
                                IsActive = z.IsActive
                            }).ToList()
                        }).FirstOrDefault();
                        db.Dispose();
                    }
                    catch (Exception exp)
                    {
                    }
                }
            });
            return result;
        }
        
        public async Task<TeamViewModel> GetByGroupId(int groupId)
        {
            var result = new TeamViewModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        string dateToday = DateTime.Now.ToString();
                        result = db.Groups.Where(x => x.Id == groupId).Select(y => new TeamViewModel
                        {
                            TotalGroup = y.Company.Groups.Count(),
                            TotalMember = y.Teams.Count,
                            TotalProfile = y.Company.ProfileComapnyGroups.Distinct().Count(),
                            CompanyName = y.Company.CompanyName,
                            Team = y.Teams.Where(x => x.IsActive == true).Select(z => new TeamMember
                            {
                                Email = z.AspNetUser.Email,
                                Name = z.FirstName + " " + z.LastName,
                                ProfileUrl = z.AspNetUser.ProfileImage == null ? null : z.AspNetUser.ProfileImage + "?" + dateToday,
                                UserId = z.UserId ?? 0,
                                TeamMemberId=z.Id
                            }).ToList()
                        }).FirstOrDefault();
                        db.Dispose();
                    }
                    catch (Exception exp)
                    {
                    }
                }
            });
            return result;
        }

        public async Task<TeamViewModel> GetByGroupIdForTaskList(int groupId)
        {
            var result = new TeamViewModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        string dateToday = DateTime.Now.ToString();
                        result = db.Groups.Where(x => x.Id == groupId).Select(y => new TeamViewModel
                        {
                            TotalGroup = y.Company.Groups.Count(),
                            TotalMember = y.Teams.Count,
                            TotalProfile = y.Company.ProfileComapnyGroups.Distinct().Count(),
                            CompanyName = y.Company.CompanyName,
                            Team = y.Teams.Select(z => new TeamMember
                            {
                                Email = z.AspNetUser.Email,
                                Name = z.FirstName + " " + z.LastName,
                                ProfileUrl = z.AspNetUser.ProfileImage == null ? null : z.AspNetUser.ProfileImage + "?" + dateToday,
                                UserId = z.UserId ?? 0,
                                TeamMemberId = z.Id
                            }).OrderBy(x => x.Name).ToList()
                        }).FirstOrDefault();
                        db.Dispose();
                    }
                    catch (Exception exp)
                    {
                    }
                }
            });
            return result;
        }
        public async Task<TeamMemberViewModel> GetByUserId(int userId, int companyId)
        {
            var result = new TeamMemberViewModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        result = db.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(y => new TeamMemberViewModel
                        {
                            Profile = new TeamMember
                            {
                                AssignedProfile = y.ProfileComapnyGroups.Where(z => z.CompanyId == companyId&&z.Teams.Any(j=>j.UserId==userId)).Count(),
                                Email = y.EmailId,
                                Name = y.FirstName + " " + y.LastName,
                                ProfileUrl = y.AspNetUser.ProfileImage,
                                UserId = y.UserId ?? 0,
                                TeamMemberId=y.Id
                            },
                            Groups = y.Groups.Select(z => new Models.Group.GroupModel
                            {
                                GroupId = z.Id,
                                GroupName = z.Name
                            }).ToList(),
                            Rights = y.Premissions.Select(z => new UserRightModel { Id = z.Id, Name = z.Name }).ToList(),
                            //VisibleReports = y.AccessLevels.Select(z => new UserReportModel { Id = z.Id, Name = z.Name }).ToList()
                            VisibleReports = y.UserAccessLevels.Select(x => new UserReportModel { Id = x.AccessLevel.Id, Name = x.AccessLevel.Name }).ToList()
                        }).FirstOrDefault();
                        db.Dispose();
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                }
            });
            return result;
        }
        public async Task<int> AddTeamMember(MemberViewModel model, int userid)
        {
            int result = 0;
            using (var db = new SocialCRMEntities())
            {
                //var teamMember = db.Teams.Where(x => x.EmailId == model.Email && x.CompanyId == model.CompanyId).FirstOrDefault(); 
                var teamMember = db.Teams.Where(x => x.EmailId == model.Email).FirstOrDefault();
                /*int membersCount = 0;
                
                var user = db.UserCompanies.ToList().Where(x => x.UserId == userid).FirstOrDefault();
                string packageName = user.Company.SubscriberType.Name;
                int allowedTeamMembers = 0;
                if(user.Company.SubscriberType.MembershipRestrictions != null && user.Company.SubscriberType.MembershipRestrictions.Count > 0)
                    allowedTeamMembers = user.Company.SubscriberType.MembershipRestrictions.Where(y => y.RestrictionTypeId == (int)RestrictionType.Team).FirstOrDefault().Count;
                
                Company comp = db.Companies.Where(x => x.id == model.CompanyId).FirstOrDefault();
                if (comp != null)
                {
                    //if (comp.SubscriberAccountType == (int)AccountSubscriberType.Trial)
                    {
                        teamMember = db.Teams.Where(x => x.EmailId == model.Email).FirstOrDefault();
                        membersCount = db.Teams.Where(x => x.CompanyId == model.CompanyId).Count();

                        if (teamMember != null)
                            throw new Exception(model.Email + " is already a member of other company");// + comp.CompanyName);
                    }
                }*/
                if (teamMember == null)
                {
                    //member.CreatedDate = DateTime.Now;
                    //member.IsActive = false;
                    //member.IsVerified = false;
                    //db.Teams.Add(member);
                    //if(model.AssignedReports!=null&&model.AssignedReports.Count>0)
                    //    var memberAccess=db.AccessLevels.Where(x=>model.AssignedReports.Contains(x.Id)).ToList();
                            

                    //if(membersCount < allowedTeamMembers) {
                    var analyticProfile = new List<AnalyticCompanyGroup>();
                    var profilePage = new List<ProfileComapnyGroup>();
                    var AccessLevels = new List<AccessLevel>();
                    var Premissions = new List<Premission>();
                    var Groups = new List<Group>();

                    if(model.IsAdmin == 0)
                    {
                        foreach (var profile in model.AssignedProfiles)
                        {
                            switch (profile.ProfileType)
                            {
                                case CRM.Common.Enum.ProfilesType.GoogleAnalytics:
                                    analyticProfile.Add(db.AnalyticCompanyGroups.Where(x => x.AnalyticId == profile.ProfileId && x.CompanyId == model.CompanyId && x.GroupId == profile.GroupId).FirstOrDefault());
                                    break;
                                case CRM.Common.Enum.ProfilesType.Page:
                                    profilePage.Add(db.ProfileComapnyGroups.Where(pp => pp.CompanyId == model.CompanyId && pp.ProfilePageId == profile.ProfileId && pp.GroupId == profile.GroupId).FirstOrDefault());
                                    break;
                                case CRM.Common.Enum.ProfilesType.Twitter:
                                case CRM.Common.Enum.ProfilesType.LinkedIn:
                                case CRM.Common.Enum.ProfilesType.Facebook:
                                case ProfilesType.Instagram:
                                    profilePage.Add(db.ProfileComapnyGroups.Where(pp => pp.CompanyId == model.CompanyId && pp.ProfileId == profile.ProfileId && pp.GroupId == profile.GroupId).FirstOrDefault());
                                    break;
                                default:
                                    break;
                            }
                        };
                        AccessLevels = (model.AssignedReports != null && model.AssignedReports.Count > 0) ? db.AccessLevels.Where(x => model.AssignedReports.Contains(x.Id)).ToList() : default(List<AccessLevel>);
                        Premissions = (model.AssignedRights != null && model.AssignedRights.Count > 0) ? db.Premissions.Where(x => model.AssignedRights.Contains(x.Id)).ToList() : default(List<Premission>);
                        Groups = (model.AssignedGroups != null && model.AssignedGroups.Count > 0) ? db.Groups.Where(x => model.AssignedGroups.Contains(x.Id)).ToList() : default(List<Group>);
                    }
                    else
                    {
                        analyticProfile = db.AnalyticCompanyGroups.Where(x => x.CompanyId == model.CompanyId).ToList();
                        profilePage = db.ProfileComapnyGroups.Where(x => x.CompanyId == model.CompanyId).ToList();
                        Groups = db.Groups.Where(x => x.CompanyId == model.CompanyId).ToList();
                        AccessLevels = db.AccessLevels.ToList();
                        Premissions = db.Premissions.ToList();
                    }

                    int loop = 1;
                    List<UserAccessLevel> lstUserAccessLevel = new List<UserAccessLevel>();
                    foreach(AccessLevel item in AccessLevels)
                    {
                        UserAccessLevel obj = new UserAccessLevel();
                        obj.AccessLevel = item;
                        obj.OrderId = loop;
                        lstUserAccessLevel.Add(obj);
                        loop++;
                    }         
                        
                    var member = new Team
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        EmailId = model.Email,
                        CompanyId = model.CompanyId,
                        IsVerified = false,
                        IsActive = false,
                        CreatedDate = DateTime.Now,
                        //AccessLevels = AccessLevels,
                        UserAccessLevels = lstUserAccessLevel,
                        Premissions = Premissions,
                        Groups = Groups,
                        AnalyticCompanyGroups = analyticProfile,
                        ProfileComapnyGroups = profilePage
                    };
                    db.Teams.Add(member);
                    foreach (var obj in member.Groups)
                    {
                        db.AddUserNotification(obj.Id, userid, "Team Member : " + model.FirstName + "" + model.LastName + " Invited in Group : " + obj.Name, "Member Invited");
                    }

                    var resultset = await db.SaveChangesAsync();
                    if (resultset > 0)
                        result = member.Id;
                    /*}
                    else
                    {
                        throw new Exception("Team Members Limit Reached for " + packageName + " Account");
                    }*/
                }
                else
                {
                    throw new Exception("Team Member with the given email address already exists in Company.");
                }
                db.Dispose();
            }
            return result;
        }
        public async Task<bool> LinkUserAccount(int teamMemberId, int userId)
        {
            var result = false;
           using (var db = new SocialCRMEntities())
           {
               var teamMember = db.Teams.Where(x=>x.Id==teamMemberId).FirstOrDefault();
               var userAccount = db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
               if (teamMember!=null&&userAccount!=null)
               {
                   teamMember.UserId = userId;
                  var resultSet= await db.SaveChangesAsync();
                  result = resultSet > 0;
               }
           }
           return result;
        }
        public async Task<TeamViewModel> GetTeamMembersNotInGroup(int companyId, int groupId)
        {
            var result = new TeamViewModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        result = db.Teams.Where(x => x.CompanyId == companyId).Select(y => new TeamViewModel
                        {
                            TotalGroup = y.Company.Groups.Count(),
                            TotalMember = y.Company.Teams.Count,
                            TotalProfile = y.Company.ProfileComapnyGroups.Distinct().Count(),
                            CompanyName = y.Company.CompanyName,
                            Team = y.Company.Teams.Where(x => x.IsActive == true).Select(z => new TeamMember
                            {
                                Email = z.AspNetUser.Email,
                                Name = z.FirstName + " " + z.LastName,
                                ProfileUrl = z.AspNetUser.ProfileImage,
                                UserId = z.UserId ?? 0,
                                IsAddedInGroup=z.Groups.Any(g=>g.Id==groupId),
                                TeamMemberId=z.Id
                            }).ToList()
                        }).FirstOrDefault();
                        result.Team.RemoveAll(t=>t.IsAddedInGroup);
                        db.Dispose();
                    }
                    catch (Exception exp)
                    {
                    }
                }
            });
            return result;
        }
        public async Task<bool> AssignGroup(List<AssignGroupModel> teamGroup)
        {
            var result = false;
            using (var db = new SocialCRMEntities())
            {
                var teamMembers = new DataTable();
                teamMembers.Columns.Add("TeammemberId", typeof(int));
                teamMembers.Columns.Add("GroupId", typeof(int));
                foreach (var row in teamGroup)
                {
                    teamMembers.Rows.Add(row.TeammemberId, row.GroupId);
                }
                try
                {
                    var profileAssignemntTable = new SqlParameter("@teamMembers", SqlDbType.Structured);
                    profileAssignemntTable.Value = teamMembers;
                    profileAssignemntTable.TypeName = "dbo.TeamMemberGroupAssignment";
                    result = db.Database.SqlQuery<bool>("exec dbo.AssignGroup @teamMembers", profileAssignemntTable).FirstOrDefault();
                    var resultset = await db.SaveChangesAsync();
                }
                catch (Exception exp)
                {
                    result = false;
                }
                return result;
            }
        }
        public async Task<TeamMemberViewModel> GetTeamMemberProfile(int teamMemberId)
        {
            var result = new TeamMemberViewModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        result = db.Teams.Where(x => x.Id == teamMemberId).Select(y => new TeamMemberViewModel
                        {
                            Profile = new TeamMember
                            {
                                Email = y.EmailId,
                                Name = y.FirstName + " " + y.LastName,
                                ProfileUrl = y.AspNetUser.ProfileImage,
                                UserId = y.UserId ?? 0,
                                TeamMemberId = y.Id
                            },
                            Groups = y.Groups.Select(z => new Models.Group.GroupModel
                            {
                                GroupId = z.Id,
                                GroupName = z.Name,
                                IsConnected = z.Teams.Any(t => t.Id == teamMemberId)
                            }).ToList(),
                            Rights = y.Premissions.Select(z => new UserRightModel
                            {
                                Id = z.Id,
                                Name = z.Name,
                                IsConnected = z.Teams.Any(t => t.Id == teamMemberId)
                            }).ToList(),
                            //VisibleReports = y.AccessLevels.Select(z => new UserReportModel
                            //{
                            //    Id = z.Id,
                            //    Name = z.Name,
                            //    IsConnected = z.Teams.Any(t => t.Id == teamMemberId)
                            //}
                            //).ToList(),
                            VisibleReports = y.UserAccessLevels.Select(z => new UserReportModel
                            {
                                Id = z.AccessLevel.Id,
                                Name = z.AccessLevel.Name,
                                IsConnected = z.TeamMemberId == teamMemberId
                            }).ToList(),
                            ConnectedProfiles = y.ProfileComapnyGroups.Select(sp => new Models.Profiles.PersonalProfile
                            {
                                IsConnected = sp.Teams.Any(t => t.Id == teamMemberId),
                                ProfileId = sp.Id,
                                ProfileName = sp.ProfileId.HasValue ? sp.Profile.Name : sp.ProfilePage.PageName,
                                ProfileUrl = sp.ProfileId.HasValue ? sp.Profile.Name : sp.ProfilePage.PageName,
                                ProfileType = (ProfilesType)(sp.ProfileId.HasValue ? sp.Profile.ProfileTypeId : sp.ProfilePage.Profile.ProfileTypeId)
                            })
                            .Concat(y.AnalyticCompanyGroups.Select(sp => new Models.Profiles.PersonalProfile
                            {
                                IsConnected = sp.Teams.Any(t => t.Id == teamMemberId),
                                ProfileId = sp.Id,
                                ProfileName = sp.UserAnalyticAccount.AccountName,
                                ProfileUrl = sp.UserAnalyticAccount.AccountUrl,
                                ProfileType = ProfilesType.GoogleAnalytics
                            })).ToList(),
                        }).FirstOrDefault();
                        db.Dispose();
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                }
            });
            return result;
        }
        public async Task<TeamMemberViewModel> GetTeamMemberProfileByGroup(int teamMemberId, int groupId)
        {
            var result = new TeamMemberViewModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        result = db.Teams.Where(x => x.Id == teamMemberId).Select(y => new TeamMemberViewModel
                        {
                            Profile = new TeamMember
                            {
                                Email = y.EmailId,
                                Name = y.FirstName + " " + y.LastName,
                                ProfileUrl = y.AspNetUser.ProfileImage,
                                UserId = y.UserId ?? 0,
                                TeamMemberId = y.Id
                            },
                            Groups = y.Groups.Where(g => g.Id == groupId).Select(z => new Models.Group.GroupModel
                            {
                                GroupId = z.Id,
                                GroupName = z.Name,
                                IsConnected = z.Teams.Any(t => t.Id == teamMemberId)
                            }).ToList(),
                            Rights = y.Premissions.Select(z => new UserRightModel
                            {
                                Id = z.Id,
                                Name = z.Name,
                                IsConnected = z.Teams.Any(t => t.Id == teamMemberId)
                            }).ToList(),
                            //VisibleReports = y.AccessLevels.Select(z => new UserReportModel
                            //{
                            //    Id = z.Id,
                            //    Name = z.Name,
                            //    IsConnected = z.Teams.Any(t => t.Id == teamMemberId)
                            //}                            
                            //).ToList(),
                            VisibleReports = y.UserAccessLevels.Select(z => new UserReportModel
                            {
                                Id = z.AccessLevel.Id,
                                Name = z.AccessLevel.Name,
                                IsConnected = z.TeamMemberId == teamMemberId
                            }).ToList(),
                            ConnectedProfiles = y.ProfileComapnyGroups.Where(p => p.GroupId == groupId).Select(sp => new Models.Profiles.PersonalProfile
                            {
                                IsConnected = sp.Teams.Any(t => t.Id == teamMemberId),
                                ProfileId = sp.Id,
                                ProfileName = sp.ProfileId.HasValue ? sp.Profile.Name : sp.ProfilePage.PageName,
                                ProfileUrl = sp.ProfileId.HasValue ? sp.Profile.Name : sp.ProfilePage.PageName,
                                ProfileType = (ProfilesType)(sp.ProfileId.HasValue ? sp.Profile.ProfileTypeId : sp.ProfilePage.Profile.ProfileTypeId)
                            })
                            .Concat(y.AnalyticCompanyGroups.Where(p => p.GroupId == groupId).Select(sp => new Models.Profiles.PersonalProfile
                            {
                                IsConnected = sp.Teams.Any(t => t.Id == teamMemberId),
                                ProfileId = sp.Id,
                                ProfileName = sp.UserAnalyticAccount.AccountName,
                                ProfileUrl = sp.UserAnalyticAccount.AccountUrl,
                                ProfileType = ProfilesType.GoogleAnalytics
                            })).ToList(),
                        }).FirstOrDefault();
                        db.Dispose();
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                }
            });
            return result;
        }
        public async Task<bool> RemoveMemberFromCompany(int teamMemberId, int companyId)
        {
            bool result=false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var teamMember = db.Teams.Where(t=>t.Id==teamMemberId&&t.CompanyId==companyId).FirstOrDefault();
                    if (teamMember != null)
                    {
                        teamMember.IsActive = false;
                        var notifs = db.UserNotifications.Where(x => db.Groups.Any(y => y.Id == x.GroupId && y.CompanyId == companyId) && x.RecipientUserId == teamMember.UserId).ToList();
                        if (notifs != null)
                        {
                            db.UserNotifications.RemoveRange(notifs);
                        }

                        await db.SaveChangesAsync();
                        result = true;
                        var emailSubject = "360 Insight CRM- User Account Deactivated";
                        var emailBody = "Dear <b>" + teamMember.FirstName + " " + teamMember.LastName +"</b>,<br/><br/>This is to notify that your account <b>" + teamMember.EmailId +"</b> has been deactivated from Company <b>" + teamMember.Company.CompanyName + "</b>.<br/><br/> In case of concerns, contact Company Group Manager.";
                        var userEmail = ConfigurationManager.AppSettings["EmailUserName"];
                        var status = new EmailHandler().SendEmail(userEmail, teamMember.EmailId, emailSubject, emailBody);
                    }
                    db.Dispose();
                }
                catch (Exception exp)
                {
                    result = false;
                }
            }
            return result;
        }

        public async Task<bool> AddMemberToCompany(int teamMemberId, int companyId)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var teamMember = db.Teams.Where(t => t.Id == teamMemberId && t.CompanyId == companyId).FirstOrDefault();
                    if (teamMember != null)
                    {
                        teamMember.IsActive = true;
                        await db.SaveChangesAsync();
                        result = true;
                        var userEmail = ConfigurationManager.AppSettings["EmailUserName"];
                        var emailSubject = "360 Insight CRM- User Account Activated";
                        var emailBody = "Dear <b>" + teamMember.FirstName + " " + teamMember.LastName + "</b>,<br/><br/>This is to notify that your account <b>" + teamMember.EmailId + "</b> has been activated in Company <b>" + teamMember.Company.CompanyName + "</b>.<br/><br/>For further detials, contact Company Group Manager.";
                        var status = new EmailHandler().SendEmail(userEmail, teamMember.EmailId, emailSubject, emailBody);
                    }
                    db.Dispose();
                }
                catch (Exception exp)
                {
                    result = false;
                }
            }
            return result;
        }

        public async Task<bool> UpdateTeamMember(MemberUpdateModel memberInfo)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                var teamMember = db.Teams.Where(g => g.Id == memberInfo.MemberId).FirstOrDefault();
                if (teamMember != null)
                {
                    teamMember.FirstName = memberInfo.FirstName;
                    teamMember.LastName = memberInfo.LastName;
                    teamMember.CreatedDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                db.Dispose();
            }
            return result;
        }

       
    }
}