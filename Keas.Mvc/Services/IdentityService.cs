using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ietws;
using Keas.Core.Domain;
using Keas.Mvc.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Keas.Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ietws.PPSDepartment;


namespace Keas.Mvc.Services
{
    public interface IIdentityService
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByKerberos(string kerb);
        Task<string> BulkLoadPeople(string ppsCode, string teamslug);
        Task<PPSDepartmentResult> GetPpsDepartment(string ppsCode);
    }

    public class IdentityService : IIdentityService
    {
        private readonly AuthSettings _authSettings;
        private readonly ApplicationDbContext _context;

        public IdentityService(IOptions<AuthSettings> authSettings, ApplicationDbContext context)
        {
            _authSettings = authSettings.Value;
            _context = context;
        }

        public async Task<User> GetByEmail(string email)
        {
            var clientws = new IetClient(_authSettings.IamKey);
            // get IAM from email
            var iamResult = await clientws.Contacts.Search(ContactSearchField.email, email);
            var iamId = iamResult.ResponseData.Results.Length > 0 ? iamResult.ResponseData.Results[0].IamId : String.Empty;
            if (String.IsNullOrWhiteSpace(iamId))
            {
                return null;
            }
            // return info for the user identified by this IAM 
            var result = await clientws.Kerberos.Search(KerberosSearchField.iamId, iamId);

            if (result.ResponseData.Results.Length > 0)
            {
                var user = new User()
                {
                    FirstName = result.ResponseData.Results[0].DFirstName,
                    LastName = result.ResponseData.Results[0].DLastName,
                    Id = result.ResponseData.Results[0].UserId,
                    Email = email,
                    Iam = iamId
                };
                return user;
            }
            return null;
        }

        public async Task<User> GetByKerberos(string kerb)
        {
            var clientws = new IetClient(_authSettings.IamKey);
            var ucdKerbResult = await clientws.Kerberos.Search(KerberosSearchField.userId, kerb);

            if (ucdKerbResult.ResponseData.Results.Length == 0)
            {
                return null;
            }

            var ucdKerbPerson = ucdKerbResult.ResponseData.Results.Single();

            // find their email
            var ucdContactResult = await clientws.Contacts.Get(ucdKerbPerson.IamId);

            var ucdContact = ucdContactResult.ResponseData.Results.First();

            return new User()
            {
                FirstName = ucdKerbPerson.DFirstName,
                LastName = ucdKerbPerson.DLastName,
                Id = ucdKerbPerson.UserId,
                Email = ucdContact.Email,
                Iam = ucdKerbPerson.IamId
            };
        }

        public async Task<PPSDepartmentResult> GetPpsDepartment(string ppsCode)
        {
            var clientws = new IetClient(_authSettings.IamKey);
            var results = await clientws.PpsDepartment.Search(PPSDepartmentSearchField.deptCode, ppsCode);
            if (results.ResponseStatus != 0)
            {
                throw new Exception("Not successful");
            }

            if (results.ResponseData.Results.Length == 0)
            {
                return null;
            }

            return results.ResponseData.Results.FirstOrDefault();
        }

        public async Task<string> BulkLoadPeople(string ppsCode, string teamslug)
        {
            int newpeople = 0;
            StringBuilder warning = new StringBuilder();
            var team = await _context.Teams.SingleAsync(t => t.Slug == teamslug);
            var clientws = new IetClient(_authSettings.IamKey);
            var iamIds = await clientws.PPSAssociations.GetIamIds(PPSAssociationsSearchField.deptCode, ppsCode);

            foreach (var id in iamIds.ResponseData.Results)
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Iam == id.IamId);
                if (user == null)
                {
                    // User not found with IamId
                    var kerbResults = await clientws.Kerberos.Search(KerberosSearchField.iamId, id.IamId);
                    var contactResult = await clientws.Contacts.Get(id.IamId);

                    if (kerbResults.ResponseData.Results.Length > 0 && contactResult.ResponseData.Results.Length > 0)
                    {
                        user = await _context.Users.SingleOrDefaultAsync(u => u.Id == kerbResults.ResponseData.Results[0].UserId);
                        if (user == null)
                        {
                            // User not found with Kerb Id either. Add user
                            var newUser = new User()
                            {
                                FirstName = kerbResults.ResponseData.Results[0].DFirstName,
                                LastName = kerbResults.ResponseData.Results[0].DLastName,
                                Id = kerbResults.ResponseData.Results[0].UserId,
                                Email = contactResult.ResponseData.Results[0].Email,
                                Iam = id.IamId
                            };
                            if (newUser.Id == null || newUser.Email == null || newUser.FirstName == null || newUser.LastName == null)
                            {
                                warning.Append("User could not be added: IAM ID: " + id.IamId.ToString() + " Name: " + newUser.FirstName + " " + newUser.LastName + " | ");
                            }
                            else
                            {
                                _context.Users.Add(newUser);
                                SavePerson(team, newUser);
                                newpeople += 1;
                            }
                        }
                        else
                        {
                            // User existed with Kerb Id, check team
                            if (!await _context.People.AnyAsync(p => p.UserId == user.Id && p.Team.Slug == teamslug))
                            {
                                SavePerson(team, user);                                
                                newpeople += 1;
                            }
                        }
                    }
                }
                else if (!await _context.People.AnyAsync(p => p.User.Iam == id.IamId && p.Team.Slug == teamslug))
                {
                    // User exists with IAM ID, but not in this team
                    SavePerson(team, user);
                    newpeople += 1;
                }
            }
            await _context.SaveChangesAsync();
            string returnMessage = newpeople.ToString() + " new people added to the team. " + warning.ToString();
            return returnMessage;
        }

        private void SavePerson(Team team, User user)
        {
            var newPerson = new Person()
            {
                User = user,
                Team = team,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
            _context.People.Add(newPerson);            
        }
    }
}
