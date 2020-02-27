using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keas.Core.Data;
using Keas.Core.Domain;
using Keas.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Keas.Mvc.Controllers.Api
{
   [Authorize(Policy = AccessCodes.Codes.AnyRole)]
    public class TagsController : SuperController
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ListTags() 
        {
            var tags = await _context.Tags.Where(x => x.Team.Slug == Team).Select(x => x.Name).OrderBy(x => x).AsNoTracking().ToListAsync();
            return Json(tags);
        }
    }
}
