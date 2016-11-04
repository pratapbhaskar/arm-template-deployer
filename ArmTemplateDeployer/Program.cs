using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmTemplateDeployer
{
    class Program
    {
        static void Main(string[] args)
        {
            var resourceManagerProvider = new ResourceManagerProvider(ConfigurationManager.AppSettings["ClientId"], ConfigurationManager.AppSettings["ClientSecret"]);
            var resourceManagementClient = resourceManagerProvider.GetResourceManagementClient(ConfigurationManager.AppSettings["SubscriptionId"],
                ConfigurationManager.AppSettings["TenantId"]);

            var deployer = new Deployer(resourceManagementClient);
            Console.WriteLine("Deployment of Azure IotHub will begin and It might take few seconds to complete");
            var output = deployer.DeployAsync("samplehub12345", "samplegroup").GetAwaiter().GetResult();

            if (output.IsSuccess)
            {
                Console.WriteLine("Deployment successful");
                Console.WriteLine($"Deployed hub name {output.Outputs["iotHubName"]["value"]}");
                Console.WriteLine($"Deployed hub keys are \n{output.Outputs["iotHubKeys"]["value"]}");
            }else
            {
                Console.WriteLine("Deployment failed");
            }
        }
    }
}
