using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Keas.Core.Data;
using Keas.Core.Domain;
using Keas.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Keas.Mvc.Controllers.Api
{
    [Authorize(Policy = AccessCodes.Codes.AnyRole)]
    [ApiController]
    [Route("api/{teamName}/spaces/[action]")]
    public class SpacesController : SuperController
    {
        private readonly ApplicationDbContext _context;

        public SpacesController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> SearchSpaces(string q)
        {
            var orgIds = await _context.FISOrgs
                .Where(f => f.Team.Slug == Team)
                .Select(x => x.OrgCode)
                .Distinct()
                .ToListAsync();

            var queryWords = q.ToLower().Split(" ").ToList();

            var space = await _context.Spaces
                .Where(x => orgIds.Contains(x.OrgId)
                    && ((!string.IsNullOrWhiteSpace(x.BldgName) && queryWords.Any(s => x.BldgName.ToLower().Contains(s)))
                        || (!string.IsNullOrWhiteSpace(x.RoomName) && queryWords.Any(s => x.RoomName.ToLower().Contains(s)))
                        || (!string.IsNullOrWhiteSpace(x.RoomNumber) && queryWords.Any(s => x.RoomNumber.ToLower().Contains(s)))))
                .AsNoTracking()
                .OrderBy(x => x.RoomNumber)
                .ToListAsync();

            return Json(space);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            //TODO clean up workstations query or integrate with list query           
            var teamId = await _context.Teams.Where(a => a.Slug == Team).Select(s => s.Id).SingleAsync();

            var sql = SpaceQueries.List;            

            var result = _context.Database.GetDbConnection().Query(sql, new { teamId });

            var spaces = result.Select(r => new {
                space = new {
                    id = r.Id,
                    deptKey = r.DeptKey,
                    deptName = r.DeptName,
                    bldgKey = r.BldgKey,
                    bldgName = r.BldgName,
                    roomKey = r.RoomKey,
                    roomNumber = r.RoomNumber,
                    roomName = r.RoomName,
                    floorKey = r.FloorKey,
                    floorName = r.FloorName,
                    roomCategoryName = r.RoomCategoryName,
                    roomCategoryCode = r.RoomCategoryCode,
                    chartNum = r.ChartNum,
                    orgId = r.OrgId,
                    source = r.Source,
                    active = r.Active,
                    sqFt = r.SqFt
                },
                id = r.Id,
                equipmentCount = r.EquipmentCount,
                keyCount = r.KeyCount,
                workstationsTotal = r.WorkstationsTotalCount,
                workstationsInUse = r.WorkstationsInUseCount,
                tags = removeDuplications(r.Tags)
            });

            return Json(spaces);
        }

        private string removeDuplications(string tags) 
        {
            return !string.IsNullOrWhiteSpace(tags) ? string.Join(",", tags.ToString().Split(',').Distinct().ToArray()) : "";
        }

        [HttpGet]
        public async Task<IActionResult> GetSpacesForKey(int keyId)
        {
            var result = await _context.KeyXSpaces
                .Where(x => x.Key.Id == keyId
                    && x.Key.Team.Slug == Team
                    && x.Key.Active)
                .Select(x => x.Space)
                .AsNoTracking()
                .ToListAsync();

            // wrap in space info object to be consistent
            var spaces = result.Select(s => new
            {
                id = s.Id,
                space = s,
            });

            return Json(spaces);
        }

        [HttpGet]
        public async Task<IActionResult> GetTagsInSpace(int spaceId)
        {
            var tags = await _context.Workstations
                .Where(x => x.Active
                    && x.SpaceId == spaceId
                    && !string.IsNullOrWhiteSpace(x.Tags))
                .Select(x => x.Tags)
                .ToArrayAsync();

            return Json(string.Join(",", tags));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var space = await _context.Spaces
                .Where(w => w.Active && w.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            return Json(space);
        }
    }
}
