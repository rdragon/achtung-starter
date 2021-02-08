using Microsoft.Azure.Management.Fluent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class ContainerManager
    {
        private readonly AzureCredentialsProvider _azureCredentialsProvider;
        private readonly IOptions<ContainerManagerOptions> _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ContainerManager> _logger;

        public ContainerManager(
            AzureCredentialsProvider azureCredentialsProvider,
            IOptions<ContainerManagerOptions> options,
            IHttpClientFactory httpClientFactory,
            ILogger<ContainerManager> logger)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = options;
            _azureCredentialsProvider = azureCredentialsProvider;
        }

        public async Task<string> CreateContainer(int serverId)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogDebug("Creating container group...");

            if (_options.Value.DryRun)
            {
                await Task.Delay(10_000);
                return "https://github.com/rdragon/achtung-starter";
            }

            var azure = Azure.Authenticate(_azureCredentialsProvider.GetAzureCredentials()).WithDefaultSubscription();
            var resourceGroupName = _options.Value.ResourceGroupName.GetSetting(173943);
            var region = (await azure.ResourceGroups.GetByNameAsync(resourceGroupName)).Region;
            var containerGroupName = GetContainerGroupName(serverId);

            var containerGroup = await azure.ContainerGroups.Define(GetContainerGroupName(serverId))
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroupName)
                .WithLinux()
                .WithPublicImageRegistryOnly()
                .WithoutVolume()
                .DefineContainerInstance(containerGroupName)
                    .WithImage(_options.Value.ContainerImageName.GetSetting(712313))
                    .WithExternalTcpPort(80)
                    .WithCpuCoreCount(_options.Value.CpuCoreCount)
                    .WithMemorySizeInGB(_options.Value.MemorySizeInGB)
                    .Attach()
                .WithDnsPrefix(containerGroupName)
                .CreateAsync();
            _logger.LogDebug("Creating container group took {seconds} seconds.", stopwatch.ElapsedMilliseconds / 1000);

            return await WaitForStarted($"http://{containerGroup.Fqdn}");
        }

        public string GetContainerGroupName(int serverId)
        {
            return $"ci-achtung-{serverId}";
        }

        private async Task<string> WaitForStarted(string url)
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;

            while (true)
            {
                try
                {
                    await _httpClientFactory.CreateClient().GetStringAsync(url, cancellationToken);

                    return url;
                }
                catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug($"Server is not yet online: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
            }
        }
    }
}
