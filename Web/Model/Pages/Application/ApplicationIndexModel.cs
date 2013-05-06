﻿using System.Collections.Generic;
using AppReadyGo.Core.QueryResults.Application;

namespace AppReadyGo.Model.Pages.Portfolio
{
    public class ApplicationIndexModel : PagingModel
    {
        public IEnumerable<ApplicationItemModel> Applications { get; set; }

        public string PortfolioDescription { get; set; }

        public int PortfolioId { get; set; }
    }

    public class ApplicationItemModel : ApplicationDataItemResult
    {
        public string Key { get; set; }

        public bool Alternate { get; set; }

        public string TargetGroup { get; set; }

        public int Time { get; set; }

        public int Clicks { get; set; }

        public int Scrolls { get; set; }

        public IEnumerable<object> AnalyticsVersions { get; set; }

        public string Icon { get; set; }
    }
}