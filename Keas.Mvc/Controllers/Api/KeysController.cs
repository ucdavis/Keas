﻿using Keas.Core.Data;
using Keas.Core.Domain;
using Keas.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Keas.Mvc.Models.KeyViewModels;

namespace Keas.Mvc.Controllers.Api
{
    [Authorize(Policy = "KeyMasterAccess")]
    public class KeysController : SuperController
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventService _eventService;

        public KeysController(ApplicationDbContext context, IEventService eventService)
        {
            _context = context;
            _eventService = eventService;
        }

        public async Task<IActionResult> Search(string q)
        {
            var comparison = StringComparison.InvariantCultureIgnoreCase;

            var keys = await _context.Keys
                .Where(x => x.Team.Slug == Team 
                        && x.Active
                        && (x.Name.StartsWith(q, comparison) || x.Code.StartsWith(q, comparison)))
                .Include(x => x.Serials)
                .Include(x => x.KeyXSpaces)
                    .ThenInclude(xs => xs.Space)
                .AsNoTracking().ToListAsync();

            return Json(keys);
        }

        // List all keys for a team
        public async Task<IActionResult> List()
        {
            var keys = await _context.Keys
                .Where(x => x.Team.Slug == Team)
                .Include(x => x.Team)
                .Include(x => x.Serials)
                    .ThenInclude(serials => serials.KeySerialAssignment)
                        .ThenInclude(assignment => assignment.Person.User)
                .AsNoTracking()
                .ToListAsync();

            return Json(keys);
        }

        // list all keys for a space
        public async Task<IActionResult> GetKeysInSpace(int spaceId)
        {
            var joins = await _context.KeyXSpaces
                .Where(x => x.Space.Id == spaceId
                        && x.Key.Team.Slug == Team
                        && x.Key.Active)
                .Include(x => x.Key)
                    .ThenInclude(key => key.Serials)
                        .ThenInclude(serials => serials.KeySerialAssignment)
                            .ThenInclude(assignment => assignment.Person.User)
                .AsNoTracking()
                .ToListAsync();

            // you can't do both select and include
            // so map after fetch
            var keys = joins.Select(k => k.Key);

            return Json(keys);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateKeyViewModel model)
        {
            // TODO Make sure user has permissions
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // create key
            var key = new Key()
            {
                Name = model.Name,
                Code = model.Code,
            };

            // get and assign team
            var team = await _context.Teams
                .SingleAsync(t => t.Slug == Team);

            key.Team = team;

            _context.Keys.Add(key);
            await _context.SaveChangesAsync();
            await _eventService.TrackCreateKey(key);

            return Json(key);
        }
        
        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromBody]UpdateKeyViewModel model)
        {
            //TODO: check permissions, make sure SN isn't edited 
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // get full object
            var key = await _context.Keys
                .Where(x => x.Team.Slug == Team)
                .Include(x => x.Team)
                .Include(x => x.Serials)
                    .ThenInclude(serials => serials.KeySerialAssignment)
                        .ThenInclude(assignment => assignment.Person.User)
                .SingleAsync(x => x.Id == id);

            key.Code = model.Code;
            key.Name = model.Name;

            await _context.SaveChangesAsync();
            await _eventService.TrackUpdateKey(key);

            return Json(key);
        }

        [HttpPost]
        public async Task<IActionResult> AssociateSpace(int id, [FromBody] AssociateKeyViewModel model)
        {
            // TODO Make sure user has permission, make sure equipment exists, makes sure equipment is in this team
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // find key
            var key = await _context.Keys
                .Where(x => x.Team.Slug == Team && x.Active)
                .Include(x => x.KeyXSpaces)
                    .ThenInclude(x => x.Space)
                .SingleAsync(x => x.Id == id);

            // find space
            var space = await _context.Spaces
                .SingleAsync(x => x.Id == model.SpaceId);

            // check for existing relationship
            var association = await _context.KeyXSpaces
                .SingleOrDefaultAsync(x => x.KeyId == id && x.SpaceId == model.SpaceId);

            if (association != null)
            {
                return BadRequest();
            }

            // create new association and save it
            association = new KeyXSpace()
            {
                KeyId = id,
                SpaceId = model.SpaceId,
            };

            _context.KeyXSpaces.Add(association);
            //await _eventService.TrackAssignKeySerial(serial);

            await _context.SaveChangesAsync();
            return Json(association);
        }

        [HttpPost]
        public async Task<IActionResult> DisassociateSpace(int id, [FromBody] DisassociateKeyViewModel model)
        {
            // TODO Make sure user has permission, make sure equipment exists, makes sure equipment is in this team
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // find key
            var key = await _context.Keys
                .Where(x => x.Team.Slug == Team && x.Active)
                .Include(x => x.KeyXSpaces)
                    .ThenInclude(x => x.Space)
                .SingleAsync(x => x.Id == id);

            // find space
            var space = await _context.Spaces
                .SingleAsync(x => x.Id == model.SpaceId);

            // find existing relationship
            var association = await _context.KeyXSpaces
                .SingleOrDefaultAsync(x => x.KeyId == id && x.SpaceId == model.SpaceId);

            if (association == null)
            {
                return BadRequest();
            }

            _context.KeyXSpaces.Remove(association);
            //await _eventService.TrackUnAssignKeySerial(serial);

            await _context.SaveChangesAsync();
            return Json(key);
        }

        public async Task<IActionResult> GetHistory(int id)
        {
            var history = await _context.Histories
                .Where(x => x.Key.Team.Slug == Team
                    && x.AssetType == "Key"
                    && x.KeyId == id)
                .OrderByDescending(x => x.ActedDate)
                .Take(5)
                .AsNoTracking()
                .ToListAsync();

            return Json(history);
        }
    }
}