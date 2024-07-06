using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialCRM.Web.Models.Admin;
using System.Threading.Tasks;
using CRM.Entities;
using CRM.Common.Enum;
using SocialCRM.Web.Models.Profiles;
using System.Data.Entity;

namespace SocialCRM.Web.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        public async Task<bool> ActivateDeactivateCompany(int companyId, int status)
        {
            var result = false;
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        var company = db.Companies.Find(companyId);
                        if (company != null)
                        {
                            company.Status = status;
                            db.Companies.Attach(company);
                            db.Entry(company).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        //public async Task<List<CompanyDetailsModel>> GetCompanyDetails()
        //{
        //    var result = new List<CompanyDetailsModel>();
        //    try
        //    {
        //        await Task.Run(() =>
        //        {
        //            using (var db = new SocialCRMEntities())
        //            {
        //                var companies = db.Companies.ToList();
        //                foreach (var company in companies)
        //                {
        //                    CompanyDetailsModel model = new CompanyDetailsModel();
        //                    model.CompanyId = company.id;
        //                    model.CompannyName = company.CompanyName;
        //                    model.CreatedDate = company.CreatedDate.ToString("dd/MM/yyyy");
        //                    model.GroupCount = company.Groups.Count;
        //                    model.TeamMemberCount = company.Teams.Count;
        //                    model.MembershipDaysRemaining = (company.CreatedDate.AddDays(30).Date - DateTime.Now.Date).Days;
        //                    model.MembershipPlan = "Trial";
        //                    model.CompanyStatus = company.Status == 1 ? "Active" : "Disabled";
        //                    model.MembershipStatus = model.MembershipDaysRemaining <=0 ? "Expired" : "Active";
        //                    model.GroupList = new List<GroupModel>();
        //                    company.Groups.ToList().ForEach(delegate (Group obj)
        //                    {
        //                        model.GroupList.Add(new GroupModel
        //                        {
        //                            GroupId = obj.Id,
        //                            GroupName = obj.Name
        //                        });
        //                    });

        //                    model.TeamList = new List<TeamMemberModel>();
        //                    company.Teams.ToList().ForEach(delegate (Team obj)
        //                    {
        //                        model.TeamList.Add(new TeamMemberModel
        //                        {
        //                            TeamId = obj.Id,
        //                            TeamName = obj.FirstName + " " + obj.LastName
        //                        });
        //                    });


        //                    model.ProfileList = new List<SocialProfileModel>();

        //                    int[] groupIdArray = company.Groups.Select(x => x.Id).ToArray();

        //                    var profiles = db.ProfileComapnyGroups.ToList().Where(x => groupIdArray.Contains(x.GroupId) && x.ProfileId.HasValue).GroupBy(g => g.ProfileId).Select(z => new ProfileModel
        //                    {
        //                        ProfileCompnayGroupId = z.FirstOrDefault().Id,
        //                        ProfileId = z.FirstOrDefault().Profile.Id,
        //                        ProfileName = z.FirstOrDefault().Profile.Name,
        //                        ProfileType = (ProfilesType)z.FirstOrDefault().Profile.ProfileTypeId,
        //                        ProfileTypeId = z.FirstOrDefault().Profile.ProfileTypeId,
        //                        CompanyId = z.FirstOrDefault().CompanyId,
        //                        ProfilePicURL = z.FirstOrDefault().Profile.ProfilePic,
        //                        AccountName = z.FirstOrDefault().Profile.UserName,
        //                        IsPage = false,
        //                        UserId = z.FirstOrDefault().Profile.ProfileExternalId,
        //                        Id = z.FirstOrDefault().Profile.Id
        //                    });

        //                    foreach (var profile in profiles)
        //                    {
        //                        model.ProfileList.Add(new SocialProfileModel
        //                        {
        //                            ProfileId = profile.Id,
        //                            ProfileName = profile.ProfileName,
        //                            ProfilePic = profile.ProfilePicURL,
        //                            ProfileUserName = profile.AccountName,
        //                            ProfileType = Convert.ToString(profile.ProfileType)
        //                        });
        //                    }



        //                    //Profile Pages
        //                    var profilePages = db.ProfileComapnyGroups.Where(x => groupIdArray.Contains(x.GroupId) && x.ProfilePageId.HasValue).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
        //                    {
        //                        ProfileCompnayGroupId = z.FirstOrDefault().Id,
        //                        ProfileId = z.FirstOrDefault().ProfilePage.Id,
        //                        ProfileName = z.FirstOrDefault().ProfilePage.PageName,
        //                        ProfileType = (ProfilesType)z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
        //                        ProfileTypeId = z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
        //                        CompanyId = z.FirstOrDefault().CompanyId,
        //                        ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
        //                        AccountName = z.FirstOrDefault().Profile.UserName,
        //                        IsPage = true,
        //                        UserId = z.FirstOrDefault().ProfilePage.ProfilePageID,
        //                        PageId = z.FirstOrDefault().ProfilePage.ProfilePageID,
        //                        Id = z.FirstOrDefault().ProfilePage.Id
        //                    }).ToList();

        //                    foreach (var profile in profilePages)
        //                    {
        //                        model.ProfileList.Add(new SocialProfileModel
        //                        {
        //                            ProfileId = profile.Id,
        //                            ProfileName = profile.ProfileName,
        //                            ProfilePic = profile.ProfilePicURL,
        //                            ProfileUserName = profile.AccountName,
        //                            ProfileType = Convert.ToString(profile.ProfileType)
        //                        });
        //                    }

        //                    result.Add(model);
        //                }

        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        result = new List<CompanyDetailsModel>();
        //    }
        //    return result;
        //}

        public async Task<List<CompanyDetailsModel>> GetCompanyDetails()
        {
            var result = new List<CompanyDetailsModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        var companies = db.Companies.OrderByDescending(x => x.CreatedDate).ToList();
                        foreach (var company in companies)
                        {
                            CompanyDetailsModel model = new CompanyDetailsModel();
                            model.CompanyId = company.id;
                            model.CompannyName = company.CompanyName;
                            model.CreatedDate = company.CreatedDate.ToString("dd/MM/yyyy");
                            model.GroupCount = company.Groups.Count;
                            model.TeamMemberCount = company.Teams.Count;
                            model.MembershipDaysRemaining = (company.CreatedDate.AddDays(30).Date - DateTime.Now.Date).Days;
                            model.MembershipPlan = "Trial";
                            model.CompanyStatus = company.Status == 1 ? "Active" : "Disabled";
                            model.MembershipStatus = model.MembershipDaysRemaining <= 0 ? "Expired" : "Active";
                            model.GroupList = new List<GroupModel>();
                            company.Groups.ToList().ForEach(delegate (Group obj)
                            {
                                model.GroupList.Add(new GroupModel
                                {
                                    GroupId = obj.Id,
                                    GroupName = obj.Name
                                });
                            });

                            model.TeamList = new List<TeamMemberModel>();
                            model.ProfileList = new List<SocialProfileModel>();

                            foreach (var grp in company.Groups)
                            {
                                foreach (var team in grp.Teams)
                                {
                                    model.TeamList.Add(new TeamMemberModel
                                    {
                                        TeamId = team.Id,
                                        TeamName = team.FirstName + " " + team.LastName,
                                        GroupId = grp.Id

                                    });
                                }

                                var profiles = db.ProfileComapnyGroups.ToList().Where(x => x.GroupId == grp.Id && x.ProfileId.HasValue).GroupBy(g => g.ProfileId).Select(z => new ProfileModel
                                {
                                    ProfileCompnayGroupId = z.FirstOrDefault().Id,
                                    ProfileId = z.FirstOrDefault().Profile.Id,
                                    ProfileName = z.FirstOrDefault().Profile.Name,
                                    ProfileType = (ProfilesType)z.FirstOrDefault().Profile.ProfileTypeId,
                                    ProfileTypeId = z.FirstOrDefault().Profile.ProfileTypeId,
                                    CompanyId = z.FirstOrDefault().CompanyId,
                                    ProfilePicURL = z.FirstOrDefault().Profile.ProfilePic,
                                    AccountName = z.FirstOrDefault().Profile.UserName,
                                    IsPage = false,
                                    UserId = z.FirstOrDefault().Profile.ProfileExternalId,
                                    Id = z.FirstOrDefault().Profile.Id
                                });

                                foreach (var profile in profiles)
                                {
                                    model.ProfileList.Add(new SocialProfileModel
                                    {
                                        ProfileId = profile.Id,
                                        ProfileName = profile.ProfileName,
                                        ProfilePic = profile.ProfilePicURL,
                                        ProfileUserName = profile.AccountName,
                                        ProfileType = Convert.ToString(profile.ProfileType),
                                        GroupId = grp.Id
                                    });
                                }

                                //Profile Pages
                                var profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == grp.Id && x.ProfilePageId.HasValue).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
                                {
                                    ProfileCompnayGroupId = z.FirstOrDefault().Id,
                                    ProfileId = z.FirstOrDefault().ProfilePage.Id,
                                    ProfileName = z.FirstOrDefault().ProfilePage.PageName,
                                    ProfileType = (ProfilesType)z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                                    ProfileTypeId = z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                                    CompanyId = z.FirstOrDefault().CompanyId,
                                    ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
                                    AccountName = z.FirstOrDefault().ProfilePage.Profile.UserName,
                                    IsPage = true,
                                    UserId = z.FirstOrDefault().ProfilePage.ProfilePageID,
                                    PageId = z.FirstOrDefault().ProfilePage.ProfilePageID,
                                    Id = z.FirstOrDefault().ProfilePage.Id
                                }).ToList();

                                foreach (var profile in profilePages)
                                {
                                    model.ProfileList.Add(new SocialProfileModel
                                    {
                                        ProfileId = profile.Id,
                                        ProfileName = profile.ProfileName,
                                        ProfilePic = profile.ProfilePicURL,
                                        ProfileUserName = profile.AccountName,
                                        ProfileType = Convert.ToString(profile.ProfileType),
                                        GroupId = grp.Id
                                    });
                                }
                            }
                            result.Add(model);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result = new List<CompanyDetailsModel>();
            }
            return result;
        }

        public async Task<PackageModel> GetMembershipPackage(int subscriberId)
        {
            PackageModel result = new PackageModel();
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        decimal[] disabledRightsArray = { 1, 2, 5 };
                        decimal[] disabledReportsArray = { 6 };
                        result = db.SubscriberTypes.Where(x => x.Id == subscriberId).Select(y => new PackageModel()
                        {
                            SubscriberId = subscriberId,
                            Permissions = y.MembershipPermissions.Where(z => !disabledRightsArray.Contains(z.PermissionId)).Select(z => new MembershipPermissionModel()
                            {
                                PermissionId = z.PermissionId,
                                PermissionName = z.Premission.Name,
                                Enabled = z.Enabled
                            }).ToList(),
                            AccessLevels = y.MembershipAccessLevels.Where(j => !disabledReportsArray.Contains(j.AccessLevelId)).Select(z => new MembershipAccessLevelModel()
                            {
                                AccessLevelId = z.AccessLevelId,
                                AccessLevelName = z.AccessLevel.Name,
                                Enabled = z.Enabled
                            }).ToList(),
                            Restrictions = y.MembershipRestrictions.Select(z => new MembershipRestrictionModel()
                            {
                                RestrictionTypeId = z.RestrictionTypeId,
                                RestrictionTypeName = z.MembershipRestrictionType.Name,
                                Count = z.Count
                            }).ToList()
                        }).FirstOrDefault();
                    }
                });
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public async Task<bool> UpdateMembershipPackage(PackageModel package)
        {
            bool result = false;
            try
            {
                using (var db = new SocialCRMEntities())
                {
                    SubscriberType existing = db.SubscriberTypes.Where(x => x.Id == package.SubscriberId).FirstOrDefault();
                    decimal[] idArray = { 1, 2, 5 };
                    package.AccessLevels.ForEach(delegate (MembershipAccessLevelModel x)
                    {
                        existing.MembershipAccessLevels.Where(y => y.AccessLevelId == x.AccessLevelId).FirstOrDefault().Enabled = x.Enabled;
                    });

                    package.Permissions.ForEach(delegate (MembershipPermissionModel x) {
                        existing.MembershipPermissions.Where(y => y.PermissionId == x.PermissionId && !idArray.Contains(x.PermissionId)).FirstOrDefault().Enabled = x.Enabled;
                    });

                    package.Restrictions.ForEach(delegate (MembershipRestrictionModel x) {
                        existing.MembershipRestrictions.Where(y => y.RestrictionTypeId == x.RestrictionTypeId).FirstOrDefault().Count = x.Count;
                    });

                    var rowcount = await db.SaveChangesAsync();
                    result = rowcount > 0;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}