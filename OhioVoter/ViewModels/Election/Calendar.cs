using OhioVoter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Election
{
    public class Calendar
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<ElectionCalendar> ElectionDates { get; set; }
    }

}