﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    public class OhioVoterDbContext : DbContext
    {
        public DbSet<Api> Apis { get; set; }
        public DbSet<BallotCandidate> BallotCandidates { get; set; }
        public DbSet<BallotHeader> BallotHeaders { get; set; }
        public DbSet<BallotIssue> BallotIssues { get; set; }
        public DbSet<BallotOffice> BallotOffices { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CertifiedCandidate> CertifiedCandidates { get; set; }
        public DbSet<StateBoardOfEducation> StateBoardOfEducations { get; set; }
        public DbSet<ElectionCandidate> ElectionCandidates { get; set; }
        public DbSet<ElectionDate> ElectionDates { get; set; }
        public DbSet<ElectionIssue> ElectionIssues { get; set; }
        public DbSet<ElectionIssuePrecinct> ElectionIssuePrecincts { get; set; }
        public DbSet<ElectionOffice> ElectionOffices { get; set; }
        public DbSet<ElectionVotingDate> ElectionVotingDates { get; set; }
        public DbSet<EmailList> EmailLists { get; set; }
        public DbSet<EmailServer> EmailServers { get; set; }
        public DbSet<HamiltonOhioVoter> HamiltonOhioVoters { get; set; }
        public DbSet<OfficeHolder> OfficeHolders { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<OhioBoardOfElection> OhioBoardOfElections { get; set; }
        public DbSet<OhioCounty> OhioCounties { get; set; }
        public DbSet<OhioLocal> OhioLocals { get; set; }
        public DbSet<OhioPrecinct> OhioPrecincts { get; set; }
        public DbSet<OhioSchoolDistrict> OhioSchoolDistricts { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<State> States { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}