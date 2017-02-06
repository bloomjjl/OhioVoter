﻿using OhioVoter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Candidate
{
    public class CandidateSummary
    {
        public int ElectionVotingDateId { get; set; }
        public DateTime VotingDate { get; set; }

        public int OfficeId { get; set; }
        public int VoteSmartOfficeId { get; set; }
        public string OfficeName { get; set; }
        public string OfficeTerm { get; set; }

        public string CertifiedCandidateId { get; set; }
        public string PartyId { get; set; }
        public string PartyName { get; set; }
        public string OfficeHolderId { get; set; }

        public int CandidateId { get; set; }
        public int VoteSmartCandidateId { get; set; }
        public string CandidateFirstName { get; set; }
        public string CandidateMiddleName { get; set; }
        public string CandidateLastName { get; set; }
        public string CandidateSuffix { get; set; }
        public string CandidateName {
            get
            {
                return string.Format("{0} {1}", CandidateFirstName, CandidateLastName);
            }
        }

        public int RunningMateId { get; set; }
        public int VoteSmartRunningMateId { get; set; }
        public string RunningMateFirstName { get; set; }
        public string RunningMateMiddleName { get; set; }
        public string RunningMateLastName { get; set; }
        public string RunningMateSuffix { get; set; }
        public string RunningMateName
        {
            get
            {
                return string.Format("{0} {1}", RunningMateFirstName, RunningMateLastName);
            }
        }
    }




}