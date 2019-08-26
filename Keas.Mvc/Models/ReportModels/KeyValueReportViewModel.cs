using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keas.Core.Data;
using Keas.Mvc.Services;
using Microsoft.EntityFrameworkCore;

namespace Keas.Mvc.Models.ReportModels
{
    public class KeyValueReportViewModel
    {

       public List<KeyValues> KeyPairs { get; set; }

        public static async Task<KeyValueReportViewModel> Create(ApplicationDbContext context, string teamSlug)
        {
            var pairs = new List<KeyValues>();
            var attributes = await context.EquipmentAttributes.Where(a => a.Equipment.Team.Slug == teamSlug).ToListAsync();

            var eav = await context.EquipmentAttributeKeys.Where(a => a.TeamId == null || a.Team.Slug == teamSlug).Select(a => a.Key).ToArrayAsync();

            foreach(var key in attributes.Select(a => a.Key).Distinct())
            {
                var list = attributes.Where(a => a.Key == key).Select(a => a.Value).Distinct().ToList();
                var keyvalue = new KeyValues(){
                    Key = key,
                    FoundInEquipmentAttributeKeys = eav.Contains(key),
                    values = list
                };
                pairs.Add(keyvalue);
            }

            var viewModel = new KeyValueReportViewModel()
            {
                KeyPairs = pairs
            };
            return viewModel;
        }

    }

    public class KeyValues
    {
        public string  Key { get; set; }

        public bool FoundInEquipmentAttributeKeys { get; set; }

        public List<string> values { get; set; }
       

        public string ValuesList
        {
            get { return string.Join(", ", values.OrderBy(x => values).ToArray()); }
        }
    }
}
