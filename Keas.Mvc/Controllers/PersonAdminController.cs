using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Keas.Core.Data;
using Keas.Core.Domain;
using Keas.Core.Models;
using Keas.Mvc.Extensions;
using Keas.Mvc.Models;
using Keas.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValidationException = CsvHelper.ValidationException;

namespace Keas.Mvc.Controllers
{
    [Authorize(Policy = AccessCodes.Codes.PersonManagerAccess)]
    public class PersonAdminController : SuperController
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IIdentityService _identityService;

        public PersonAdminController(ApplicationDbContext context, INotificationService notificationService, IIdentityService identityService)
        {
            _context = context;
            _notificationService = notificationService;
            _identityService = identityService;
        }

        public async Task<IActionResult> BulkEdit()
        {
            var model = new BulkEditModel();
            await PopulateBulkEdit(model);


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> BulkEdit(BulkEditModel model)
        {
            var updatedCount = 0;
            var skippedCount = 0;

            if (string.IsNullOrWhiteSpace(model.Ids))
            {
                ErrorMessage = "Must select at least one person to update.";
                await PopulateBulkEdit(model);
                return View(model);
            }
            var ids = model.Ids.Split(",").Select(a => int.Parse(a)).ToArray();
            var persons = await _context.People.Include(a => a.Supervisor).Where(a => ids.Contains(a.Id)).ToListAsync();

            if (model.DeleteUsers)
            {
                foreach (var person in persons)
                {
                    if (await _context.AccessAssignments.AnyAsync(a => a.PersonId == person.Id) ||
                        await _context.KeySerialAssignments.AnyAsync(a => a.PersonId == person.Id) ||
                        await _context.EquipmentAssignments.AnyAsync(a => a.PersonId == person.Id) ||
                        await _context.WorkstationAssignments.AnyAsync(a => a.PersonId == person.Id))
                    {
                        skippedCount++;
                        continue;
                    }

                    if (await _context.TeamPermissions.AnyAsync(a => a.TeamId == person.TeamId && a.UserId == person.UserId))
                    {
                        skippedCount++;
                        continue;
                    }

                    if (person.UserId == User.Identity.Name)
                    {
                        skippedCount++;
                        continue;
                    }

                    await _notificationService.PersonUpdated(person, null, Team, User.GetNameClaim(), User.Identity.Name, PersonNotification.Actions.Deactivated, "From Bulk Edit");
                    updatedCount++;
                    person.Active = false;
                }
            }
            else
            {
                int? supervisorId = null;
                if (!model.UpdateCategory && !model.UpdateEndDate && !model.UpdateStartDate && !model.UpdateSupervisorEmail && !model.UpdateTags)
                {
                    ErrorMessage = "You have not selected anything to update.";
                    await PopulateBulkEdit(model);
                    return View(model);
                }
                if (model.UpdateSupervisorEmail)
                {
                    if (!string.IsNullOrWhiteSpace(model.SupervisorEmail))
                    {
                        var supervisor = await _context.People.Where(a => a.Team.Slug == Team && a.Email == model.SupervisorEmail).FirstOrDefaultAsync();
                        if (supervisor == null)
                        {
                            ErrorMessage = "Supervisor not found.";
                            await PopulateBulkEdit(model);
                            return View(model);
                        }
                        else
                        {
                            supervisorId = supervisor.Id;
                        }
                    }
                }
                foreach (var person in persons)
                {

                    if (model.UpdateCategory)
                    {
                        if (PersonCategories.Types.Contains(model.Category))
                        {
                            person.Category = PersonCategories.Types.Single(a =>
                                a.Equals(model.Category.Trim(), StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            person.Category = string.Empty;
                        }
                    }

                    if (model.UpdateStartDate)
                    {
                        person.StartDate = model.StartDate;
                    }

                    if (model.UpdateEndDate)
                    {
                        person.EndDate = model.EndDate;
                    }

                    if (model.UpdateSupervisorEmail)
                    {
                        person.SupervisorId = supervisorId;
                    }

                    if (model.UpdateTags)
                    {
                        if (model.SelectedTags == null || model.SelectedTags.Length == 0)
                        {
                            person.Tags = string.Empty;
                        }
                        else
                        {
                            person.Tags = string.Join(",", model.SelectedTags);
                        }
                    }
                    updatedCount++;
                }
            }

            _context.UpdateRange(persons);
            await _context.SaveChangesAsync();
            await PopulateBulkEdit(model);
            Message = $"{ids.Length} people selected. Updated: {updatedCount} Skipped: {skippedCount}";
            return View(model);
        }

        public IActionResult UploadPeople()
        {
            var model = new PeopleImportModel
            {
                UpdateExistingUsers = false,
                ImportResult = new List<PeopleImportResult>()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPeople(PeopleImportModel model)
        {
            var resultsView = new List<PeopleImportResult>();
            if (model.File == null || model.File.Length <= 0)
            {
                ErrorMessage = "No file, or an empty file selected.";
                return View(model);
            }

            var userIdentity = User.Identity.Name;
            var userName = User.GetNameClaim();
            var team = await _context.Teams.FirstAsync(t => t.Slug == Team);
            using (var reader = new StreamReader(model.File.OpenReadStream()))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = (string header, int index) => header.ToLower().Replace(" ", string.Empty),
                TrimOptions = TrimOptions.Trim
            }))
            {
                var record = new PeopleImport();
                var records = csv.EnumerateRecords(record);

                try
                {
                    csv.Read();
                    csv.ReadHeader();
                    csv.ValidateHeader(typeof(PeopleImport));
                }
                catch (HeaderValidationException e)
                {
                    var firstSentence = e.Message.Split('.');
                    ErrorMessage = firstSentence.FirstOrDefault() ?? "Error Detected";
                    return View(model);
                }


                var counter = 1;
                try  //Or, I could make the start and end dates strings and then try to parse them into dates
                {
                    foreach (var r in records)
                    {
                        counter++;
                        var importResult = new PeopleImportResult(r);
                        importResult.LineNumber = csv.Context.Row;
                        importResult.Success = true;

                        if (string.IsNullOrWhiteSpace(r.OverrideEmail))
                        {
                            r.OverrideEmail = null; //Need this or the validation of the email will be triggered for an empty string
                        }

                        try
                        {
                            //validate
                            ICollection<ValidationResult> valResults = new List<ValidationResult>();
                            var context = new ValidationContext(r);
                            Validator.TryValidateObject(r, context, valResults, true); //Need to validate all properties
                            if (valResults.Count > 0)
                            {
                                foreach (var validationResult in valResults)
                                {
                                    importResult.ErrorMessage.Add(validationResult.ErrorMessage);
                                }
                            }
                        }
                        catch (ValidationException)
                        {
                            importResult.ErrorMessage.Add("Validation Exception for this row.");
                        }

                        if (importResult.ErrorMessage.Count > 0)
                        {
                            importResult.Success = false;
                        }
                        else
                        {
                            if (!model.UpdateExistingUsers && await _context.People.AnyAsync(a => a.Active && a.UserId == r.KerbId && a.TeamId == team.Id))
                            {
                                importResult.Messages.Add($"KerbId {r.KerbId} Already active in team, no changes made.");
                                importResult.Success = false;
                            }
                        }


                        if (importResult.Success)
                        {
                            Person person = null;
                            if (model.UpdateExistingUsers)
                            {
                                person = await _context.People.FirstOrDefaultAsync(a => a.Active && a.UserId == r.KerbId && a.TeamId == team.Id);
                            }
                            try
                            {
                                if (person == null)
                                {
                                    var personResult = await _identityService.GetOrCreatePersonFromKerberos(r.KerbId, team.Id, team, userName, userIdentity, "CSV People Import");
                                    person = personResult.Person;
                                }

                                await PopulatePerson(r, person, importResult, team);

                                await _context.SaveChangesAsync();
                            }
                            catch (Exception)
                            {
                                person = null;
                                importResult.Success = false;
                                importResult.ErrorMessage.Add($"!!!!!!!!!!!!!THERE IS A PROBLEM WITH KerbUser {r.KerbId} PLEASE CONTACT PEAKS HELP with this User ID.!!!!!!!!!!!!!!!");
                            }

                            if (person == null)
                            {
                                importResult.Success = false;
                                importResult.ErrorMessage.Add($"KerbId not found.");
                            }
                        }


                        resultsView.Add(importResult);
                    }
                }
                catch (Exception)
                {
                    counter++;
                    var importResult = new PeopleImportResult(new PeopleImport());
                    importResult.Success = false;
                    importResult.LineNumber = counter;
                    importResult.ErrorMessage.Add("There is a problem with this row preventing it from being imported. Maybe an invalid date format. Import Halted.");
                    resultsView.Add(importResult);
                    ErrorMessage = "Import halted.";
                }

            }

            if (resultsView.Count <= 0)
            {
                ErrorMessage = "No rows in file";
            }

            model.ImportResult = resultsView;

            return View(model);
        }

        private async Task PopulatePerson(PeopleImport r, Person person, PeopleImportResult importResult, Team team)
        {
            if (!string.IsNullOrWhiteSpace(r.OverrideFirstName))
            {
                person.FirstName = r.OverrideFirstName;
            }

            if (!string.IsNullOrWhiteSpace(r.OverrideLastName))
            {
                person.LastName = r.OverrideLastName;
            }

            if (!string.IsNullOrWhiteSpace(r.OverrideEmail))
            {
                person.Email = r.OverrideEmail;
            }

            if (r.StartDate.HasValue)
            {
                person.StartDate = r.StartDate.Value;
            }

            if (r.EndDate.HasValue)
            {
                person.EndDate = r.EndDate.Value;
            }

            if (!string.IsNullOrWhiteSpace(r.Category))
            {
                if (PersonCategories.Types.Contains(r.Category.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    person.Category = PersonCategories.Types.Single(a =>
                        a.Equals(r.Category.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    importResult.Messages.Add("Warning, supplied category not found. Value not set.");
                }
            }

            if (!string.IsNullOrWhiteSpace(r.SupervisorKerbId))
            {
                try
                {
                    var superGuy = await _context.People.FirstOrDefaultAsync(a => a.UserId == r.SupervisorKerbId.Trim() && a.TeamId == team.Id);
                    if (superGuy == null)
                    {
                        importResult.Messages.Add("Supplied SuperviorId not found in team.");
                    }
                    else
                    {
                        person.Supervisor = superGuy;
                        _context.Attach(person.Supervisor);
                    }
                }
                catch (Exception)
                {
                    importResult.Messages.Add("An error happened trying to set the supervisor. Value not set.");
                }

            }

            if (!string.IsNullOrWhiteSpace(r.OverrideTitle))
            {
                person.Title = r.OverrideTitle;
            }

            if (!string.IsNullOrWhiteSpace(r.HomePhone))
            {
                try
                {
                    person.HomePhone = r.HomePhone;
                }
                catch (Exception)
                {
                    person.HomePhone = null;
                }

                if (string.IsNullOrWhiteSpace(person.HomePhone))
                {
                    importResult.Messages.Add("Supplied Home Phone is not a valid phone number format. Value not set.");
                }
            }
            if (!string.IsNullOrWhiteSpace(r.TeamPhone))
            {
                try
                {
                    person.TeamPhone = r.TeamPhone;
                }
                catch (Exception)
                {
                    person.TeamPhone = null;
                }

                if (string.IsNullOrWhiteSpace(person.TeamPhone))
                {
                    importResult.Messages.Add("Supplied Team Phone is not a valid phone number format. Value not set.");
                }
            }

            if (!string.IsNullOrWhiteSpace(r.Notes))
            {
                person.Notes = r.Notes;
            }

            person.Tags = string.IsNullOrWhiteSpace(r.Tags) ? "Imported" : $"{r.Tags},Imported";

            return;
        }

        private async Task PopulateBulkEdit(BulkEditModel model)
        {
            model.BulkPersons = await _context.ExtendedPersonViews.Where(a => a.Slug == Team).Select(a => new PersonBulkEdit
            {
                Id = a.Id,
                UserId = a.UserId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                Tags = a.Tags,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Category = a.Category,
                SupervisorName = a.SupervisorId == null ? null : $"{a.S_FirstName} {a.S_LastName} ({a.S_Email})",
                AddedDate = a.LastAddDate,
            }).ToListAsync();
            model.CategoryChoices = new List<string>();
            model.CategoryChoices.Add("-- Not Set --");
            model.CategoryChoices.AddRange(PersonCategories.Types);
            model.Tags = await _context.Tags.Where(a => a.Team.Slug == Team).Select(a => a.Name).ToListAsync();
            model.DeleteUsers = false;
        }

    }
}
