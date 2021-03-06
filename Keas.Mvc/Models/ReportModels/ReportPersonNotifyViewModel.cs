using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Keas.Core.Domain;

namespace Keas.Mvc.Models.ReportModels
{
    public class ReportPersonNotifyViewModel
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public IList<PersonNotification> PersonNotifications { get; set; }
    }
}
