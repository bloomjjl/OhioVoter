using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace OhioVoter.Models
{
    public class OhioVoterDbContext : DbContext
    {
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CertifiedCandidate> CertifiedCandidates { get; set; }
        public DbSet<ElectionCandidate> ElectionCandidates { get; set; }
        public DbSet<ElectionDate> ElectionDates { get; set; }
        public DbSet<ElectionIssuePrecinct> ElectionIssuePrecincts { get; set; }
        public DbSet<ElectionIssue> ElectionIssues { get; set; }
        public DbSet<ElectionOffice> ElectionOffices { get; set; }
        public DbSet<ElectionVotingDate> ElectionVotingDates { get; set; }
        public DbSet<HamiltonOhioVoter> HamiltonOhioVoters { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<OfficeHolder> OfficeHolders { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<OhioBoardOfElection> OhioBoardOfElections { get; set; }
        public DbSet<OhioCounty> OhioCounties { get; set; }
        public DbSet<OhioLocal> OhioLocals { get; set; }
        public DbSet<OhioPrecinct> OhioPrecincts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}