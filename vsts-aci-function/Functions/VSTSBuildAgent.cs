using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using System.Configuration;
using System.Collections.Generic;

namespace Functions
{
    public static class VSTSBuildAgent
    {
        private static IAzure _azure = GetAzure();

        [FunctionName("StartVSTSBuildAgent")]
        public static async Task<HttpResponseMessage> StartVSTSBuildAgenttAsync([HttpTrigger(AuthorizationLevel.Function, "GET", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var containerImageFullUrl = $"{ConfigurationManager.AppSettings["AZURE_AGENT_IMAGE_REGISTRY"]}{ConfigurationManager.AppSettings["AZURE_AGENT_IMAGE_REPOSITORY"]}";
            var resourceGroup = await _azure.ResourceGroups.GetByNameAsync(ConfigurationManager.AppSettings["AZURE_RESOURCE_GROUP_NAME"]);
            var env = new Dictionary<string, string>
            {
                { "VSTS_TOKEN", ConfigurationManager.AppSettings["VSTS_TOKEN"] },
                { "VSTS_POOL", ConfigurationManager.AppSettings["VSTS_POOL"] },
                { "VSTS_AGENT", ConfigurationManager.AppSettings["VSTS_AGENT_NAME"] },
                { "VSTS_ACCOUNT", ConfigurationManager.AppSettings["VSTS_ACCOUNT"] }
            };

            var containerGroup = await _azure.ContainerGroups.Define(ConfigurationManager.AppSettings["AZURE_AGENT_CONTAINER_GROUP_NAME"])
                .WithRegion(resourceGroup.RegionName)
                .WithExistingResourceGroup(resourceGroup)
                .WithLinux()
                .WithPrivateImageRegistry(ConfigurationManager.AppSettings["AZURE_AGENT_IMAGE_REGISTRY"], ConfigurationManager.AppSettings["AZURE_AD_APP_ID"], ConfigurationManager.AppSettings["AZURE_AD_APP_KEY"])
                .WithoutVolume()
                .DefineContainerInstance(ConfigurationManager.AppSettings["AZURE_AGENT_CONTAINER_INSTANCE_NAME"])
                    .WithImage(containerImageFullUrl)
                    .WithoutPorts()
                    .WithEnvironmentVariables(env)
                    .WithCpuCoreCount(Convert.ToDouble(ConfigurationManager.AppSettings["AZURE_AGENT_CONTAINER_CPU_CORE_COUNT"]))
                    .WithMemorySizeInGB(Convert.ToDouble(ConfigurationManager.AppSettings["AZURE_AGENT_CONTAINER_MEMORY_SIZE_GB"]))
                    .Attach()
                .WithRestartPolicy(ContainerGroupRestartPolicy.Never)
                .CreateAsync();

            return req.CreateResponse(HttpStatusCode.OK, "VSTS agent is running");
        }

        [FunctionName("StopVSTSBuildAgent")]
        public static async Task<HttpResponseMessage> StopVSTSBuildAgenttAsync([HttpTrigger(AuthorizationLevel.Function, "GET", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            await _azure.ContainerGroups.DeleteByResourceGroupAsync(ConfigurationManager.AppSettings["AZURE_RESOURCE_GROUP_NAME"], ConfigurationManager.AppSettings["AZURE_AGENT_CONTAINER_GROUP_NAME"]);
            return req.CreateResponse(HttpStatusCode.OK, "VSTS agent has been removed");
        }

        private static async Task<string> GetNameAsync(HttpRequestMessage req, string key)
        {
            // parse query parameter
            var name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Equals(q.Key, key, StringComparison.OrdinalIgnoreCase))
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            return name ?? data?.name;
        }

        private static IAzure GetAzure()
        {
            var tenantId = ConfigurationManager.AppSettings["AZURE_TENANT_ID"];
            var sp = new ServicePrincipalLoginInformation
            {
                ClientId = ConfigurationManager.AppSettings["AZURE_AD_APP_ID"],
                ClientSecret = ConfigurationManager.AppSettings["AZURE_AD_APP_KEY"]
            };
            return Azure.Authenticate(new AzureCredentials(sp, tenantId, AzureEnvironment.AzureGlobalCloud)).WithDefaultSubscription();
        }
    }
}
