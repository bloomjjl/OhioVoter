using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    [Table("tblElectionCandidate")]
    public class ElectionCandidate
    {
        public ElectionCandidate()
        {

        }

        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("ElectionVotingDate_Id")]
        public int ElectionVotingDateId { get; set; }

        [Required]
        [Column("ElectionOffice_Id")]
        public int ElectionOfficeId { get; set; }

        [Required]
        [Column("Candidate_Id")]
        public int CandidateId { get; set; }

        [Required]
        [Column("CertifiedCandidate_Id")]
        public string CertifiedCandidateId { get; set; }

        [Column("Party_Id")]
        public string PartyId { get; set; }

        [Column("OfficeHolder_Id")]
        public string OfficeHolderId { get; set; }

        public int RunningMateId { get; set; }




        [ForeignKey("ElectionVotingDateId")]
        public virtual ElectionVotingDate ElectionVotingDate { get; set; }

        [ForeignKey("ElectionOfficeId")]
        public virtual ElectionOffice ElectionOffice { get; set; }

        [ForeignKey("CandidateId")]
        public virtual Candidate Candidate { get; set; }

        [ForeignKey("CertifiedCandidateId")]
        public virtual CertifiedCandidate CertifiedCandidate { get; set; }

        [ForeignKey("PartyId")]
        public virtual Party Party { get; set; }

        [ForeignKey("OfficeHolderId")]
        public virtual OfficeHolder OfficeHolder { get; set; }
    }
}