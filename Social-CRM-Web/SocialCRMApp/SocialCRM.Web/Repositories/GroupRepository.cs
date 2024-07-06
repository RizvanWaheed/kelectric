using CRM.Common.Enum;
using CRM.Entities;
using SocialCRM.Web.Models.Group;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        public async Task<int> AddGroup(Group groupInfo, int currentUserId)
        {
            int result = 0;
            using (var db = new SocialCRMEntities())
            {
                result = groupInfo.Id;
                var isExist = db.Groups.Where(x => x.Name == groupInfo.Name && x.CompanyId == groupInfo.CompanyId).FirstOrDefault();
                /*var user = db.UserCompanies.ToList().Where(x => x.UserId == currentUserId).FirstOrDefault();
                string packageName = user.Company.SubscriberType.Name;
                int allowedGroups = 0;
                if (user.Company.SubscriberType.MembershipRestrictions != null && user.Company.SubscriberType.MembershipRestrictions.Count > 0)
                    allowedGroups = user.Company.SubscriberType.MembershipRestrictions.Where(y => y.RestrictionTypeId == (int)RestrictionType.Group).FirstOrDefault().Count;*/
                if (isExist == null)
                {
                    /*int brandsCount = 0;
                    Company comp = db.Companies.Where(x => x.id == groupInfo.CompanyId).FirstOrDefault();
                    if (comp != null)
                    {
                        //if (comp.SubscriberAccountType == (int)AccountSubscriberType.Trial)
                        {
                            brandsCount = db.Groups.Where(x => x.CompanyId == groupInfo.CompanyId).Count();
                        }
                    }

                    if(brandsCount < allowedGroups)
                    {*/
                        groupInfo.Teams = new HashSet<Team>();
                        groupInfo.Teams = db.Teams.Where(t => t.CompanyId == groupInfo.CompanyId && t.AspNetUser.AspNetRoles.Any(y => y.Name == "Admin")).ToList();
                        //groupInfo.Teams.Add(db.Teams.Where(t => t.UserId == currentUserId && t.CompanyId == groupInfo.CompanyId).FirstOrDefault());
                        groupInfo.UserNotificationSettings = new HashSet<UserNotificationSetting>();
                        foreach(Team team in groupInfo.Teams)
                        {
                            team.ProfileComapnyGroups = groupInfo.ProfileComapnyGroups;
                            team.AnalyticCompanyGroups = groupInfo.AnalyticCompanyGroups;
                            groupInfo.UserNotificationSettings.Add(new UserNotificationSetting { CompanyId = groupInfo.CompanyId, IsEmailEnable = true, IsNotificationEnable = true, UserId = team.AspNetUser.Id });
                        }
                        
                        db.Groups.Add(groupInfo);
                        /*}
                        else
                        {
                            throw new Exception("Group Limit Reached for " + packageName + " Account");
                        }*/
                    }
                else
                {
                    throw new Exception("Group with the name '" + groupInfo.Name + "' already exists in the company.");
                }
                await db.SaveChangesAsync();
                db.Dispose();
            }
            return result;
        }
        public async Task<bool> UpdateGroup(GroupModel groupInfo)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                var group = db.Groups.Where(g => g.Id == groupInfo.GroupId).FirstOrDefault();
                var isExist = db.Groups.Where(x => x.Name == groupInfo.GroupName && x.CompanyId == group.CompanyId).FirstOrDefault();
                if (isExist == null)
                {
                    if (group != null)
                    {
                        group.Name = groupInfo.GroupName;
                        group.CreatedDate = DateTime.Now;
                    }
                }
                else
                {
                    throw new Exception("Group with the name '" + groupInfo.GroupName + "' already exists in the company.");
                }
                await db.SaveChangesAsync();
                db.Dispose();
            }
            return result;
        }
        public async Task<string> DeleteGroup(int groupId)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                var isExist = db.Groups.Where(x => x.Id == groupId).FirstOrDefault();
                if (isExist != null)
                {
                    //isExist.IsActive = false;
                    try
                    {
                        //Checking if the Group is the last that contains the base Social Profile Account
                        var data = (from BB in (
                        (from PCG0 in db.ProfileComapnyGroups
                         where
                           PCG0.GroupId == groupId &&
                           PCG0.Profile.IsBaseAccount == true
                         select new
                         {
                             PCG0.ProfileId
                         }))
                                   from PCG in db.ProfileComapnyGroups
                                   where BB.ProfileId == PCG.ProfileId &&
                                         PCG.ProfilePageId == null
                                   group PCG by new
                                   {
                                       PCG.ProfileId
                                   } into g
                                   where g.Count() == 1
                                   select new
                                   {
                                       Occurence = g.Count(),
                                       g.Key.ProfileId
                                   }).ToList();
                        if (data.Count() > 0)
                            return "EBUN";

                        var isDeleted = db.DeleteGroupById(groupId).FirstOrDefault();
                        var resultset = await db.SaveChangesAsync();
                        //result = isDeleted.HasValue ? isDeleted.Value : resultset>0;
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        result = false;
                    }
                }
                db.Dispose();
            }
            return result.ToString();
        }
        public async Task<List<GroupViewModel>> GetUserGroups(int userId)
        {
            List<GroupViewModel> result = new List<GroupViewModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var usergroups = db.Teams.Where(x => x.UserId == userId).Select(y => y.Groups.Select(z => new GroupViewModel { GroupId = z.Id, GroupName = z.Name, CompanyId = z.CompanyId }));
                    foreach (var companygrp in usergroups)
                    {
                        foreach (var grp in companygrp)
                            result.Add(grp);
                    }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<GroupViewModel>> GetUserCompanyGroups(int userId)
        {
            List<GroupViewModel> result = new List<GroupViewModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var usergroups = db.Teams.Where(x => x.UserId == userId && (x.IsVerified == false || x.IsActive == true)).Select(y => y.Groups.Select(z => new GroupViewModel
                    {
                        GroupId = z.Id,
                        GroupName = z.Name,
                        CompanyId = z.CompanyId,
                        CompanyName = z.Company.CompanyName
                    }));
                    foreach (var companygrp in usergroups)
                    {
                        foreach (var grp in companygrp)
                            result.Add(grp);
                    }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<GroupViewModel>> GetCompanyGroups(int companyId)
        {
            List<GroupViewModel> result = null;
            await Task.Run(() =>
           {
               using (var db = new SocialCRMEntities())
               {
                   result = db.Groups.Where(x => x.CompanyId == companyId).Select(z => new GroupViewModel { TotalMembers = z.Teams.Count, TotalProfiles = z.ProfileComapnyGroups.Count, GroupId = z.Id, GroupName = z.Name, CompanyId = z.CompanyId }).ToList();
                   db.Dispose();
               }
           });
            return result;
        }
        public async Task<GroupViewModel> GetGroupById(int groupId)
        {
            GroupViewModel result = new GroupViewModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.Groups.Where(x => x.Id == groupId).Select(y => new GroupViewModel { TotalProfiles = y.ProfileComapnyGroups.Count(), TotalMembers = y.Teams.Count(), GroupId = y.Id, GroupName = y.Name, CompanyId = y.CompanyId }).FirstOrDefault();
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<bool> RemoveMember(int teamMemberId, int groupId)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    foreach (var data in db.RemoveMemberFromGroup(teamMemberId, groupId))
                    {
                        result = data.Value;
                    };
                    // result = succes == 1 ?? false;
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }
    }
}