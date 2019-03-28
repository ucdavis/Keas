using Keas.Core.Data;
using Keas.Core.Domain;
using Keas.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Keas.Core.Models;
using Keas.Mvc.Models.KeySerialViewModels;
using Keas.Mvc.Extensions;

namespace Keas.Mvc.Controllers.Api
{
    [Authorize(Policy = AccessCodes.Codes.KeyMasterAccess)]
    public class KeySerialsController : SuperController
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventService _eventService;

        public KeySerialsController(ApplicationDbContext context, IEventService eventService)
        {
            _context = context;
            _eventService = eventService;
        }

        //Return Serials instead????
        public async Task<IActionResult> Search(string q)
        {
            // break out the query into terms
            var terms = q.Split(' ');

            var comparison = StringComparison.InvariantCultureIgnoreCase;

            var query = _context.KeySerials
                .Where(x => x.Key.Team.Slug == Team && x.Key.Active && x.Active);

            foreach (var term in terms)
            {
                query = query.Where(x => x.Key.Name.StartsWith(term, comparison)
                                        || x.Key.Code.StartsWith(term, comparison)
                                        || x.Number.StartsWith(term, comparison));
            }

            var keySerials = await query
                .Include(x => x.Key)
                .Include(x => x.KeySerialAssignment)
                .OrderBy(x => x.KeySerialAssignment != null).ThenBy(x => x.Number)
                .AsNoTracking()
                .ToListAsync();

            return Json(keySerials);
        }
     public async Task<IActionResult> SearchInKey(int keyId, string q)
        {
            // break out the query into terms
            var terms = q.Split(' ');

            var comparison = StringComparison.InvariantCultureIgnoreCase;

            var query = _context.KeySerials
                .Where(x => x.Key.Team.Slug == Team && x.Key.Active && x.Active && x.Key.Id == keyId);

            foreach (var term in terms)
            {
                query = query.Where(x => x.Key.Name.StartsWith(term, comparison)
                                        || x.Key.Code.StartsWith(term, comparison)
                                        || x.Number.StartsWith(term, comparison));
            }

            var keySerials = await query
                .Include(x => x.Key)
                .Include(x => x.KeySerialAssignment)
                .OrderBy(x => x.KeySerialAssignment != null).ThenBy(x => x.Number)
                .AsNoTracking()
                .ToListAsync();

            return Json(keySerials);
        }

        // list all serials for a key
        public async Task<IActionResult> GetForKey(int keyid)
        {
            var keys = await _context.KeySerials
                .Where(x => x.Key.Team.Slug == Team && x.Key.Active)
                .Where(x => x.KeyId == keyid)
                .Include(x => x.Key)
                .Include(s => s.KeySerialAssignment)
                    .ThenInclude(assignment => assignment.Person)
                .AsNoTracking()
                .ToListAsync();

            return Json(keys);
        }

        // get all the key serials attached to a person
        public async Task<IActionResult> GetForPerson(int personId)
        {
            var keySerials = await _context.KeySerials
                .Where(x => x.Key.Team.Slug == Team)
                .Where(x => x.KeySerialAssignment.PersonId == personId)
                .Include(x => x.Key)
                .Include(x => x.KeySerialAssignment)
                    .ThenInclude(assingment => assingment.Person)
                .AsNoTracking()
                .ToArrayAsync();

            return Json(keySerials);
        }

        public async Task<IActionResult> Create([FromBody] CreateKeySerialViewModel model)
        {
            // TODO Make sure user has permissions
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // get key
            var key = await _context.Keys
                .Where(x => x.Team.Slug == Team && x.Active)
                .Include(k => k.Serials)
                .SingleOrDefaultAsync(k => k.Id == model.KeyId);

            if (key == null)
            {
                return BadRequest();
            }

            // check for duplicate serial
            if (key.Serials.Any(s => s.Number.Equals(model.Number.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest();
            }

            // create key serial
            var keySerial = new KeySerial
            {
                KeyId    = key.Id,
                TeamId   = key.TeamId,
                Name     = model.Number.Trim(),
                Number   = model.Number.Trim(),
                Notes    = model.Notes, 
                Status   = model.Status ?? "Active",
            };

            // add key serial
            key.Serials.Add(keySerial);
            await _context.SaveChangesAsync();
            
            //await _eventService.TrackCreateKeySerial(keySerial);

            return Json(keySerial);
        }

        public async Task<IActionResult> Update(int id, [FromBody] UpdateKeySerialViewModel model)
        {
            // TODO Make sure user has permissions
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // get key serial
            var keySerial = await _context.KeySerials
                .Where(x => x.Key.Team.Slug == Team && x.Active)
                .Include(x => x.KeySerialAssignment)
                    .ThenInclude(assingment => assingment.Person)
                .SingleOrDefaultAsync(k => k.Id == id);

            if (keySerial == null)
            {
                return BadRequest();
            }

            // get key
            var key = await _context.Keys
                .Include(k => k.Serials)
                .SingleOrDefaultAsync(k => k.Id == keySerial.KeyId);

            // check for duplicate serial to target number
            if (key.Serials.Any(s => s.Id != id && s.Number.Equals(model.Number.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest();
            }

            // update key serial properties
            keySerial.Number = model.Number.Trim();
            //TODO: Do we want to update the Name too? When we create it, we make Number and Name the same.
            keySerial.Status = model.Status;
            keySerial.Notes = model.Notes;

            await _context.SaveChangesAsync();

            //await _eventService.TrackCreateKeySerial(key);

            return Json(keySerial);
        }

        public async Task<IActionResult> Assign([FromBody] AssignKeySerialViewModel model)
        {
            // TODO Make sure user has permission, make sure equipment exists, makes sure equipment is in this team
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // find serial
            var serial = await _context.KeySerials
                .Where(x => x.Key.Team.Slug == Team && x.Active)
                .Include(x => x.KeySerialAssignment)
                .Include(x => x.Key)
                    .ThenInclude(k => k.Team)
                .SingleAsync(x => x.Id == model.KeySerialId);

            // find person
            var person = await _context.People
                .SingleAsync(p => p.Id == model.PersonId);


            // check for existing assignment and update
            if (serial.KeySerialAssignment != null)
            {
                serial.KeySerialAssignment.ExpiresAt = model.ExpiresAt;
                serial.KeySerialAssignment.RequestedById =  User.Identity.Name;
                serial.KeySerialAssignment.RequestedByName = User.GetNameClaim();

                _context.KeySerialAssignments.Update(serial.KeySerialAssignment);
                await _eventService.TrackAssignmentUpdatedKeySerial(serial);
            }
            else 
            {
                var assignment = new KeySerialAssignment
                {
                    KeySerial   = serial,
                    KeySerialId = serial.Id,
                    Person      = person,
                    PersonId    = person.Id,
                    ExpiresAt   = model.ExpiresAt,
                    RequestedById = User.Identity.Name,
                    RequestedByName = User.GetNameClaim()
                };

                // create, associate, and track
                
                _context.KeySerialAssignments.Add(assignment);
                await _context.SaveChangesAsync();
                serial.KeySerialAssignment = assignment;
                serial.KeySerialAssignmentId = assignment.Id;
                await _eventService.TrackAssignKeySerial(serial);
            }

            await _context.SaveChangesAsync();
            return Json(serial);
        }

        public async Task<IActionResult> Revoke(int id)
        {
            //TODO: check permissions

            // find keyserial
            var keySerial = await _context.KeySerials
                .Where(x => x.Key.Team.Slug == Team)
                .Include(s => s.Key)
                .ThenInclude(s => s.Team)
                .Include(s => s.KeySerialAssignment)
                .ThenInclude(s => s.Person)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (keySerial == null)
            {
                return NotFound();
            }

            if (keySerial.KeySerialAssignment == null)
            {
                return BadRequest();
            }

            await _eventService.TrackUnAssignKeySerial(keySerial);

            // clear out assignment
            var assignment = keySerial.KeySerialAssignment;
            _context.KeySerialAssignments.Remove(assignment);

            await _context.SaveChangesAsync();
            

            // return unassigned key serial
            return Json(keySerial);
        }

        public async Task<IActionResult> GetHistory(int id)
        {
            var history = await _context.Histories
                .Where(x => x.KeySerial.Key.Team.Slug == Team
                        && x.AssetType == "KeySerial"
                        && x.KeySerial.Id == id)
                .OrderByDescending(x => x.ActedDate)
                .Take(5)
                .AsNoTracking()
                .ToListAsync();

            return Json(history);
        }
    }
}
