using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class AzureCredentialsProviderOptions
    {
        public string? ClientId { get; set; }

        public string? ClientSecret { get; set; }

        public string? TenantId { get; set; }
    }
}
