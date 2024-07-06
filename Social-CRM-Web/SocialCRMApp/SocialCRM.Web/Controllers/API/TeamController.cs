using CRM.Common;
using SocialCRM.Web.Models.Team;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CRM.Common.Extensions;
using SocialCRM.Web.Models.User;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Configuration;
using SocialCRM.Web.Models.Company;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/Team")]
    public class TeamController : ApiController
    {
        ITeamRepository repository;
        ICompanyRepository companyRepo;
        string baseUrl = ConfigurationManager.AppSettings["WebUrl"];
        public TeamController(ITeamRepository repo, ICompanyRepository crepo)
        {
            repository = repo;
            companyRepo = crepo;
        }
        [HttpPost]
        public async Task<IHttpActionResult> Register(MemberViewModel model)
        {
            int userid = User.Identity.GetUserId<int>();

            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.Email))
            {
                return BadRequest(ModelState);
            }

            var teamMemberId = await repository.AddTeamMember(model, userid);
            int userAccountId = 0;

            if (teamMemberId > 0)
            {
                var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var isUserExist = userManager.FindByEmail(model.Email);
                var Company = await companyRepo.GetCompany(model.CompanyId);

                if (isUserExist == null)
                {
                    //Generate Notificaiton
                    var member = new RegisterViewModel { Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Password = CodeGenerator.ConfirmationString(8), ConfirmationCode = CodeGenerator.ConfirmationString(10) };
                    var user = new ApplicationUser { ConfirmationCode = string.Empty, EmailConfirmed = false, UserName = member.Email, Email = member.Email, Name = string.Concat(member.FirstName, " ", member.LastName) };//, ProfileImage = model.ImageUrl };
                    IdentityResult result = await userManager.CreateAsync(user, member.Password);
                    if (result.Succeeded)
                    {
                        //Adding this User to Requested Company.
                        if(model.IsAdmin == 1)
                            userManager.AddToRole(user.Id, "Admin");
                        userAccountId = user.Id;
                        await companyRepo.AddMember(userAccountId, model.CompanyId, (int)CRM.Common.Enum.CompanyTypes.Assigned);
                        await companyRepo.EnableGroupNotification(userAccountId, model);
                        var message = new IdentityMessage
                        {
                            Destination = model.Email,
                            Body = string.Format("Dear <b>{0}</b>,<br/><br/>You are invited as a team member in 360 Insight CRM Company <b>{1}</b>.<br/><br/>Use the following information to log in the system and activate company in your account: <br/><br/>Website Url: <b>{2}</b><br/>UserName: <b>{3}</b><br/>Password: <b>{4}</b><br/><br/>Once logged in the system, you are allowed to change your password.<br/><br/> Sincerely,<br/>360 Insight CRM Team", user.Name, Company.CompanyName, baseUrl, member.Email, member.Password),
                            Subject = "360 Insight CRM- Invitation to Access"
                        };
                        await userManager.EmailService.SendAsync(message);
                    }
                }
                else
                {
                    //Adding this User to Requested Company.
                    userAccountId = isUserExist.Id;
                    await companyRepo.AddMember(userAccountId, model.CompanyId, (int)CRM.Common.Enum.CompanyTypes.Assigned);
                    await companyRepo.EnableGroupNotification(userAccountId, model);
                    //Send password and link

                    var message = new IdentityMessage
                    {
                        Destination = model.Email,
                        Body = string.Format("Dear <b>{0}</b>,<br/>You are invited as a team member in 360 Insight CRM Company <b>{1}</b>.<br/><br/>Credentials to log in the system will be the same as provided earlier. Please log in the system to activate company in your account on clicking the below link:<br/><br/>Website Url: <b>{2}</b><br/><br/>Contact Company Group Manager in case of any concern.<br/><br/> Sincerely,<br/>Social CRM Team", string.Concat(model.FirstName, " ", model.LastName), Company.CompanyName, baseUrl),
                        Subject = "360 Insight CRM- Invitation to Access"
                    };
                    await userManager.EmailService.SendAsync(message);

                }
                //Link user Account.
                await repository.LinkUserAccount(teamMemberId, userAccountId);
            }

            return Ok(teamMemberId);
        }
        /// <summary>
        /// This Api is used to update teamMember it updated member at company level.
        /// it add/Remove the selected Profiles,Rights,Premissions,groups
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(RequestStatus))]
        [Route("UpdateMemberInCompany")]
        public async Task<IHttpActionResult> Update(TeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var status = await repository.UpdateGroup(new CRM.Entities.Group { Name = model.GroupName, CompanyId = model.CompanyId, IsActive = model.IsActive });
            return Ok();
        }
        [HttpPost]
        [ResponseType(typeof(RequestStatus))]
        [Route("UpdateTeamMember")]
        public async Task<IHttpActionResult> UpdateTeamMember(MemberUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.UpdateTeamMember(model);
            return Ok(status);
        }
        /// <summary>
        /// This api is used to update the teammember in specified group.
        /// it update user profile only.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(RequestStatus))]
        [Route("UpdateMemberInGroup")]
        public async Task<IHttpActionResult> UpdateMemberInGroup(TeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var status = await repository.UpdateGroup(new CRM.Entities.Group { Name = model.GroupName, CompanyId = model.CompanyId, IsActive = model.IsActive });
            return Ok();
        }
        /// <summary>
        /// Remove Memeber from Company.
        /// we take CompanyId  to cross verify that memeber exist in specified company or Not.
        /// </summary>
        /// <param name="teamMemberId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [ResponseType(typeof(RequestStatus))]
        [HttpGet]
        [Route("RemoveMemberFromCompany/{teamMemberId}/{companyId}")]
        public async Task<IHttpActionResult> RemoveMemberFromCompany(int teamMemberId, int companyId)
        {
            if (teamMemberId == 0 || companyId == 0)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.RemoveMemberFromCompany(teamMemberId, companyId);
            return Ok(status);
        }

        [ResponseType(typeof(RequestStatus))]
        [HttpGet]
        [Route("AddMemberToCompany/{teamMemberId}/{companyId}")]
        public async Task<IHttpActionResult> AddMemberToCompany(int teamMemberId, int companyId)
        {
            if (teamMemberId == 0 || companyId == 0)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.AddMemberToCompany(teamMemberId, companyId);
            return Ok(status);
        }

        [HttpGet]
        [ResponseType(typeof(List<TeamViewModel>))]
        public async Task<IHttpActionResult> GetMemberByUserId(int id, int param)
        {
            var userId = id;
            var companyId = param;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetByUserId(userId, companyId);
            return Ok(status);
        }

        [HttpGet]
        [ResponseType(typeof(List<TeamViewModel>))]
        public async Task<IHttpActionResult> GetByCompanyId(int id)
        {
            var companyId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetByCompanyId(companyId);
            return Ok(status);
        }


        [HttpGet]
        [Route("GetByCompanyIdAndGroupId/{companyId}/{groupId}")]
        public async Task<IHttpActionResult> GetByCompanyIdAndGroupId(int companyId, int groupId)
        {
            var company = companyId;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetByGroupId(groupId);
            return Ok(status);
        }

        [HttpGet]
        [ResponseType(typeof(TeamViewModel))]
        public async Task<IHttpActionResult> GetByGroupId(int id)
        {
            var userId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var teamMembers = await repository.GetByGroupId(userId);
            teamMembers.Team.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.ProfileUrl))
                    x.ProfileUrl = System.Web.HttpContext.Current.Request.GetFullUrl(x.ProfileUrl);
            });
            return Ok(teamMembers);
        }

        [HttpGet]
        [ResponseType(typeof(TeamViewModel))]
        public async Task<IHttpActionResult> GetByGroupIdForTaskList(int id)
        {
            var userId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var teamMembers = await repository.GetByGroupIdForTaskList(userId);
            teamMembers.Team.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.ProfileUrl))
                    x.ProfileUrl = System.Web.HttpContext.Current.Request.GetFullUrl(x.ProfileUrl);
            });
            return Ok(teamMembers);
        }

        /// <summary>
        /// This api return all team members of company and show the connected start of groupId.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(TeamViewModel))]
        [Route("GetTeamMembersNotInGroup/{companyId}/{groupId}")]
        public async Task<IHttpActionResult> GetTeamMembersNotInGroup(int companyId, int groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTeamMembersNotInGroup(companyId, groupId);
            return Ok(status);
        }

        [HttpPost]
        [Route("AssignGroup")]
        public async Task<IHttpActionResult> AssignGroup(List<AssignGroupModel> teamGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.AssignGroup(teamGroup);
            return Ok(status);
        }

        /// <summary>
        /// This API take teammember id and return its complete profile
        /// including it Premission,it report access, it assigned profiles
        /// its assigned group and also show its not assigned info too.
        /// </summary>
        /// <param name="teamMemberId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTeamMemberProfile/{teamMemberId}")]
        public async Task<IHttpActionResult> GetTeamMemberProfile(int teamMemberId)
        {
            if (teamMemberId == 0)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTeamMemberProfile(teamMemberId);
            return Ok(status);
        }

        /// <summary>
        /// This API is used to get teamMemberProfile in specified group.
        /// </summary>
        /// <param name="teamMemberId"></param>
        /// <param name="groupId"></param>
        /// <returns>TeamMemberProfileJson</returns>
        [HttpGet]
        [Route("GetTeamMemberProfileByGroup/{teamMemberId}/{groupId}")]
        public async Task<IHttpActionResult> GetTeamMemberProfileByGroup(int teamMemberId, int groupId)
        {
            if (teamMemberId == 0 || groupId == 0)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTeamMemberProfileByGroup(teamMemberId, groupId);
            return Ok(status);
        }
    }
}
