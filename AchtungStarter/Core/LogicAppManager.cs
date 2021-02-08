using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Logic;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class LogicAppManager
    {
        private readonly AzureCredentialsProvider _azureCredentialsProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<LogicAppManagerOptions> _options;

        public LogicAppManager(
            AzureCredentialsProvider azureCredentialsProvider,
            IHttpClientFactory httpClientFactory,
            IOptions<LogicAppManagerOptions> options)
        {
            _options = options;
            _httpClientFactory = httpClientFactory;
            _azureCredentialsProvider = azureCredentialsProvider;
        }

        public async Task<string> ScheduleShutDown(DateTimeOffset shutDownTime, string containerGroupName)
        {
            if (_options.Value.DryRun)
            {
                await Task.Delay(500);

                return "dummy";
            }

            var url = _options.Value.LogicAppTriggerUrl.GetSetting(164271);
            var content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    delaySeconds = Math.Max(1, shutDownTime.Subtract(DateTimeOffset.Now).TotalSeconds),
                    containerGroupName,
                }),
                Encoding.UTF8,
                "application/json");
            var response = await _httpClientFactory.CreateClient().PostAsync(url, content);
            await response.RequireSuccessStatusCode();

            return response.Headers.GetValues("x-ms-workflow-run-id").First();
        }

        public async Task CancelShutDown(string runName)
        {
            if (_options.Value.DryRun)
            {
                await Task.Delay(500);
                return;
            }

            var client = new LogicManagementClient(_azureCredentialsProvider.GetAzureCredentials())
            {
                SubscriptionId = _options.Value.SubscriptionId.GetSetting(811226)
            };

            await client.WorkflowRuns.CancelAsync(
                resourceGroupName: _options.Value.ResourceGroupName.GetSetting(666564),
                workflowName: _options.Value.WorkflowName.GetSetting(873644),
                runName: runName);
        }
    }
}
