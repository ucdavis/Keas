using Keas.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Keas.Core.Extensions;

namespace Keas.Mvc.Models
{
    public class TeamAdminMembersListModel
    {

        public Team Team { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public static TeamAdminMembersListModel Create(Team team, string userId)
        {
            var viewModel = new TeamAdminMembersListModel()
            {
                Team = team,
                UserRoles = new List<UserRole>()
            };

            if (userId != null)
            {
                team.TeamPermissions= team.TeamPermissions.Where(tp => tp.UserId == userId).ToList();
            }
            foreach (var teamPermission in team.TeamPermissions)
            {
                if (viewModel.UserRoles.Any(a => a.User.Id == teamPermission.User.Id))
                {
                    viewModel.UserRoles.Single(a => a.User.Id == teamPermission.User.Id).Roles
                        .Add(teamPermission.Role);
                }
                else
                {
                    viewModel.UserRoles.Add(new UserRole(teamPermission));
                }
            }
            viewModel.UserRoles = viewModel.UserRoles.OrderBy(u => u.User.LastName).ThenBy(u => u.User.FirstName).ToList();
            return viewModel;
        }

    }

    public class UserRole
    {
        public User User { get; set; }

        public IList<Role> Roles { get; set; }

        public UserRole(TeamPermission teamPermission)
        {
            User = teamPermission.User;
            Roles = new List<Role>();
            Roles.Add(teamPermission.Role);
        }

        public string RolesList
        {
            get { return string.Join(", ", Roles.OrderBy(x => x.Name).Select(a => a.Name.Humanize(LetterCasing.Sentence).SafeHumanizeTitle()).ToArray()); }
        }
    }
}
