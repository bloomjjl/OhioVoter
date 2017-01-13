using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    public class ElectionCalendar
    {
        [Required]
        [Key]
        public int ElectionCalendarId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Description can not be more than 200 characters.")]
        public string Description { get; set; }
    }
}