﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppReadyGo.Core.QueryResults.Applications
{
    public class PublishResult
    {
        public IEnumerable<KeyValueResult> Countries { get; set; }

        public string ApplicationName { get; set; }
    }
}