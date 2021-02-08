# Achtung Starter
A web app to start and stop an [Achtung, die Kurve!](https://github.com/marcusklaas/Achtung--die-Kurve-) server.

Visit the app here: [https://achtung-starter.azurewebsites.net](https://achtung-starter.azurewebsites.net). The app might take a minute to load if it wasn't already running.

## Goal
Host an [Achtung, die Kurve!](https://github.com/marcusklaas/Achtung--die-Kurve-) server while making minimal costs. The server doesn't need to run 24/7.

## Overview
Achtung Starter is built using [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor). It creates an [Azure Container Instance](https://azure.microsoft.com/en-us/services/container-instances/) that runs [a Docker image](https://hub.docker.com/r/olrslp/achtung) of the Achtung server.

The Achtung server is automatically shut down after a certain amount of minutes. For this an [Azure Logic App](https://azure.microsoft.com/en-us/services/logic-apps/) is used. The reason for using a logic app instead of the Achtung Starter app, is that the Achtung Starter app is allowed to shutdown after a certain (possibly smaller) amount of minutes. This is for example the case when you use a free Azure App Service Plan to host Achtung Starter. A logic app doesn't have this problem.

You can postpone the shutdown by pressing a button inside the Achtung Starter app (e.g. the button "Add 30 minutes"). This is the only way to postpone the shutdown. It doesn't matter whether there is any activity on the Achtung server. The shutdown takes place at the specified time, even if there are still games being played.

## Configuration
The following values can be configured through `AchtungStarter/appsettings.config` or by using environment variables with the prefix `AchtungStarter_` (e.g. `AchtungStarter_StateLocation`). For development also `AchtungStarter/appsettings.Development.config` and the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) can be used.
| Name | Description |
| --- | --- |
| StateLocation | The path to the file that stores the state of the application (e.g. whether the Achtung server is online). |
| ContainerImageName | The name of the Docker image. Only public image registries are supported. | 
| CpuCoreCount | The number of CPU cores assigned to the Achtung server. |
| MemorySizeInGB | The memory size assigned to the Achtung server. |
| LogicAppTriggerUrl | The URL that triggers the logic app. |
| WorkflowName | The name of the logic app. |
| ResourceGroupName | The resource group that contains the logic app and that also will be used for the container instances. |
| TenantId | See [https://docs.microsoft.com/en-us/dotnet/azure/authentication](https://docs.microsoft.com/en-us/dotnet/azure/authentication) |
| ClientId | See [https://docs.microsoft.com/en-us/dotnet/azure/authentication](https://docs.microsoft.com/en-us/dotnet/azure/authentication) |
| ClientSecret | See [https://docs.microsoft.com/en-us/dotnet/azure/authentication](https://docs.microsoft.com/en-us/dotnet/azure/authentication) |
| SubscriptionId | The ID of your Azure subscription. |
| SlackWebhookUrl | Optional. A [Slack](https://slack.com/) webhook URL. If set then all warnings and errors are posted to this URL.
| DryRun | If true then no calls to Azure will be made. Can be used to test the UI. |

## Quick start
Follow these steps to build and run Achtung Starter.
- Install the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) and sign in to an Azure account.
- Run the following command to create the logic app (replace `<deploymentName>` and `<resourceGroupName>`):
```
az deployment group create \
  --name <deploymentName> \
  --resource-group <resourceGroupName> \
  --template-file logic-app-template.json \
  --parameters workflowName=logic-achtung
```
- Using the Azure Portal, add a "Delete a container group" step to the logic app. Use the variable `containerGroupName` as Container Group Name. Save the logic app.
- Fill in all empty values in `AchtungStarter/appsettings.Development.json`. Instead of modifying this file you can also use the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets).
- Run `dotnet run -p AchtungStarter`
- Browse to `https://localhost:5001`


## Why not use Azure App Services to host Achtung?
An [Azure App Service](https://azure.microsoft.com/en-us/services/app-service/) also supports Docker images and websockets. Therefore, you can also run the Achtung server in an app service. However, I was not able to get it running in an app service using the free plan, only using a paid plan. All paid plans are more expensive than using container instances for Achtung. This is because the Achtung server is expected not to be used often.

I don't know why the Achtung server didn't run on the free plan. The free plan should also support websockets, but I was not able to establish a websocket connection.




