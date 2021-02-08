using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class AzureCredentialsProvider
    {
        private readonly IOptions<AzureCredentialsProviderOptions> _options;

        public AzureCredentialsProvider(IOptions<AzureCredentialsProviderOptions> options)
        {
            _options = options;
        }

        public AzureCredentials GetAzureCredentials()
        {
            return SdkContext.AzureCredentialsFactory.FromServicePrincipal(
                _options.Value.ClientId.GetSetting(176422),
                _options.Value.ClientSecret.GetSetting(215346),
                _options.Value.TenantId.GetSetting(711864),
                AzureEnvironment.AzureGlobalCloud);
        }
    }
}
