using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Newtonsoft.Json.Linq;

namespace ArmTemplateDeployer
{
    public class Deployer
    {

        private static string GetEmbeddedResourceAsString(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"ArmTemplateDeployer.{fileName}.json";
            string result = string.Empty;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        private IResourceManagementClient resourceManagementClient;
        
        public Deployer(IResourceManagementClient resourceManagementClient)
        {
            this.resourceManagementClient = resourceManagementClient;
        }

        public async Task<DeploymentResponse> DeployAsync(string iotHubName, string resourceGroupName)
        {
            var parameters = new Dictionary<string, Dictionary<string, object>>
            {
                {"iotHubName", new Dictionary<string, object> { {"value", iotHubName} } }
            };

            var resourceGroupResponse = await resourceManagementClient.ResourceGroups.CreateOrUpdateAsync(resourceGroupName,
                new ResourceGroup { Location = "SouthEast asia" });
            
            var deploymentExtended = await resourceManagementClient.Deployments.CreateOrUpdateAsync(resourceGroupName, iotHubName,
                new Deployment
                {
                    Properties = new DeploymentProperties
                    {
                        Mode = DeploymentMode.Incremental,
                        Template = JObject.Parse(GetEmbeddedResourceAsString("iotHubDeploy")),
                        Parameters = parameters
                    }
                });
            var hasSucceeded = deploymentExtended.Properties.ProvisioningState == "Succeeded";
            return !hasSucceeded ? new DeploymentResponse { IsSuccess = hasSucceeded }
                    : new DeploymentResponse
                    {
                        IsSuccess = hasSucceeded,
                        Outputs = deploymentExtended.Properties.Outputs != null ?
                            JObject.Parse(deploymentExtended.Properties.Outputs.ToString()).ToObject<Dictionary<string, Dictionary<string, object>>>()
                            : new Dictionary<string, Dictionary<string, object>>()
                    };
        }
    }
}
