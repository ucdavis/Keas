using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keas.Core.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace Keas.Mvc.Services
{
    public interface IRolesSessionsManager
    {
        Task<string[]> GetTeamRoleNames(string slug);
    }

    public class RolesSessionsManager : IRolesSessionsManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private const string RolesSessionKey = "TeamRolesSessionKey";


        public RolesSessionsManager(IHttpContextAccessor contextAccessor, ApplicationDbContext dbContext)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Will return empty array if no roles.
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<string[]> GetTeamRoleNames(string slug)
        {
            var roleContainer = await GetRoleContainer(slug);
            
            return roleContainer.Teams.First(a => a.TeamName == slug).TeamRoles;
        }

        private async Task<RoleContainer> GetRoleContainer(string slug)
        {
            var userId = _contextAccessor.HttpContext.User.Identity.Name;

            var sessionResult = _contextAccessor.HttpContext.Session.GetString(RolesSessionKey);

            var roleContainer = new RoleContainer();

            if (sessionResult == null)
            {
                roleContainer = new RoleContainer();
                roleContainer.UserId = userId;
            }
            else
            {
                try
                {
                    roleContainer = JsonConvert.DeserializeObject<RoleContainer>(sessionResult);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    roleContainer = new RoleContainer();
                }

                if (roleContainer.UserId != userId)
                {
                    _contextAccessor.HttpContext.Session.Remove(RolesSessionKey);
                    roleContainer = new RoleContainer();
                    roleContainer.UserId = userId;
                }
            }

            var team = roleContainer.Teams.FirstOrDefault(a => a.TeamName == slug);

            if (team == null)
            {
                team = new Team
                {
                    TeamName = slug,
                    TeamRoles = await _dbContext.TeamPermissions
                        .Where(a => a.Team.Slug == slug && a.UserId == userId)
                        .Select(a => a.Role.Name).ToArrayAsync(),
                };
                roleContainer.Teams.Add(team);

                _contextAccessor.HttpContext.Session.SetString(RolesSessionKey, JsonConvert.SerializeObject(roleContainer));
            }

            return roleContainer;
        }


        public class RoleContainer
        {
            public RoleContainer()
            {
                Teams = new List<Team>();
            }
            public string UserId { get; set; }
            public List<Team> Teams { get; set; }
        }

        public class Team
        {
            public string TeamName { get; set; }
            public string[] TeamRoles { get; set; }
        }

    }
}
