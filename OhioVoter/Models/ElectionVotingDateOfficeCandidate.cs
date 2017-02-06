using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    public class ElectionVotingDateOfficeCandidate
    {
        [Required]
        [Key]
        [Column(Order = 0)]
        public int ElectionVotingDateId { get; set; }

        [Required]
        [Key]
        [Column(Order = 1)]
        public int OfficeId { get; set; }

        [Required]
        [Key]
        [Column(Order = 2)]
        public int CandidateId { get; set; }

        [Required]
        public string CertifiedCandidateId { get; set; }

        public string PartyId { get; set; }

        public string OfficeHolderId { get; set; }

        public int RunningMateId { get; set; }
    }
}