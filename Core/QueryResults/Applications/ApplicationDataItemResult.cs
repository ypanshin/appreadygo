﻿using System;
using AppReadyGo.Core.Entities;

namespace AppReadyGo.Core.QueryResults.Applications
{
    public class ApplicationDataItemResult : ApplicationResult
    {
        public int Visits { get; set; }

        public string Type { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public string IconExt { get; set; }

        public bool Published { get; set; }

        public int Downloaded { get; set; }

        public bool HasScreens { get; set; }
    }
}
