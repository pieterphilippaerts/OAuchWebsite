﻿using OAuch.Database.Entities;
using System.Collections.Generic;

namespace OAuch.ViewModels {
    public class DashboardViewModel : IMenuInformation {
        public IList<Site>? Sites { get; set; }
        public Site? ActiveSite { get; set; }
        public PageType PageType { get; set; }

        //public Dictionary<Site, DateTime> SiteDeadlines { get; set; }
        public List<SiteResult> SiteResults { get; set; }
        public bool IsFirstLogin { get; set; }
    }
}