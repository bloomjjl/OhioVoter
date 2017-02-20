using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Issue
{
    public class IssueDisplayViewModel
    {
        public int DateId { get; set; }
        public int CountyId { get; set; }
        public int CommunityId { get; set; }
        public List<Issue> Issues { get; set; }
    }

    public class Issue
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Requirement { get; set; }
        public string Details { get; set; }
        public string FullTextLink { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
    }

}