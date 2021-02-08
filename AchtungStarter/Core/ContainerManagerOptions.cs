using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class ContainerManagerOptions
    {
        public string? ResourceGroupName { get; set; }

        public string? ContainerImageName { get; set; }

        public int CpuCoreCount { get; set; }

        public double MemorySizeInGB { get; set; }

        public bool DryRun { get; set; }
    }
}
