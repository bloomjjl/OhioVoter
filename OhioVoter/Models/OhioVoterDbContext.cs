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
        public DbSet<ElectionCandidate> ElectionCandidates { get; set; }
        public DbSet<ElectionDate> ElectionDates { get; set; }
        public DbSet<ElectionIssue> ElectionIssues { get; set; }
        public DbSet<ElectionIssuePrecinct> ElectionIssuePrecincts { get; set; }
        public DbSet<ElectionOffice> ElectionOffices { get; set; }
        public DbSet<ElectionParty> ElectionParties { get; set; }
        public DbSet<ElectionPrecinct> ElectionPrecincts { get; set; }
        public DbSet<ElectionVotingDateOfficeCandidate> ElectionVotingDateOfficeCandidates { get; set; }
        public DbSet<ElectionVotingDate> ElectionVotingDates { get; set; }
        public DbSet<ElectionOfficeHolder> OfficeHolders { get; set; }
        public DbSet<OhioCounty> OhioCounties { get; set; }
        public DbSet<OhioLocal> OhioLocals { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}