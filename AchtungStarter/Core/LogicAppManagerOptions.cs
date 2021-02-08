using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class LogicAppManagerOptions
    {
        public string? LogicAppTriggerUrl { get; set; }

        public string? SubscriptionId { get; set; }

        public string? ResourceGroupName { get; set; }

        public string? WorkflowName { get; set; }

        public bool DryRun { get; set; }
    }
}
