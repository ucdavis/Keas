﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keas.Core.Data;
using Keas.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Keas.Mvc.Controllers
{
    public class TagsController : SuperController
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "DepartmentAdminAccess")]
        // GET: Tags
        public async Task<ActionResult> Index()
        {
            var model = await _context.Tags.Where(t => t.Team.Name == Team).OrderBy(t=> t.Name).ToListAsync();
            return View(model);
        }

        [Authorize(Policy = "DepartmentAdminAccess")]
        // GET: Tags/Create
        public ActionResult Create()
        {
            return View();
        }
        
        [Authorize(Policy = "DepartmentAdminAccess")]
        // POST: Tags/Create
        [HttpPost]
        public async Task<ActionResult> Create( Tag newTag)
        {
            if (ModelState.IsValid)
            {
                var team = await _context.Teams.FirstAsync(t => t.Name == Team);
                newTag.Team = team;
                _context.Tags.Add(newTag);
                await _context.SaveChangesAsync();
                Message = "Tag created.";
                return RedirectToAction(nameof(Index));
            }
            Message = "An error occurred. Tag could not be created.";
            return View();
        }
        
        [Authorize(Policy = "DepartmentAdminAccess")]
        // GET: Tags/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var tag = await _context.Tags.SingleAsync(t => t.Team.Name == Team && t.Id == id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }
        
        [Authorize(Policy = "DepartmentAdminAccess")]
        // POST: Tags/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Tag updatedTag)
        {
            if (id != updatedTag.Id)
            {
                return NotFound();
            }
            var tagToUpdate = await _context.Tags.SingleAsync(t => t.Id == id && t.Team.Name==Team);
            if (await TryUpdateModelAsync<Tag>(tagToUpdate, "", t => t.Name))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    Message = "Tag updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Unable to save changes.");
                }
            }
            return View(updatedTag);
        }
        
        [Authorize(Policy = "DepartmentAdminAccess")]
        // GET: Tags/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var tag = await _context.Tags.SingleAsync(t => t.Team.Name == Team && t.Id == id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }
        
        [Authorize(Policy = "DepartmentAdminAccess")]
        // POST: Tags/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, Tag deleteTag)
        {
            var tagToDelete = await _context.Tags.SingleAsync(t => t.Team.Name == Team && t.Id == id);
            _context.Remove(tagToDelete);
            await _context.SaveChangesAsync();
            Message = "Tag deleted.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ListTags() 
        {
            var tags = await _context.Tags.Where(x => x.Team.Name == Team).Select(x => x.Name).AsNoTracking().ToListAsync();
            return Json(tags);
        }
    }
}