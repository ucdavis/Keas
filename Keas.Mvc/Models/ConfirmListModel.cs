﻿using System.Collections.Generic;
using Keas.Core.Data;
using Keas.Core.Domain;
using System.Linq;
using System.Threading.Tasks;
using Keas.Mvc.Services;
using Microsoft.EntityFrameworkCore;

namespace Keas.Mvc.Models
{
    public class ConfirmListModel
    {
        public List<Serial> Serials { get; set; }
        public List<Equipment> Equipment { get; set; }
        public List<Workstation> Workstations { get; set; }

       
        public static async Task<ConfirmListModel> Create(ApplicationDbContext context, Person person)
        {
            var viewModel = new ConfirmListModel
            {
                Serials = await context.Serials.Include(s=> s.Key).ThenInclude(k=> k.KeyXSpaces).ThenInclude(kxs=> kxs.Space).Where(s=> !s.Assignment.IsConfirmed && s.Assignment.Person== person).AsNoTracking().ToListAsync(),
                Equipment = await context.Equipment.Include(e=> e.Space).Where(e => !e.Assignment.IsConfirmed && e.Assignment.Person==person).AsNoTracking().ToListAsync(),
                Workstations = await context.Workstations.Include(w=> w.Space).Where(w=> !w.Assignment.IsConfirmed && w.Assignment.Person==person).AsNoTracking().ToListAsync()
            };
            
            return viewModel;
        }

    }
}
