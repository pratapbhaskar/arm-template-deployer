using System.Collections.Generic;

namespace ArmTemplateDeployer
{
    public class DeploymentResponse
    {
        public bool IsSuccess { get; set; }
        public Dictionary<string, Dictionary<string,object>> Outputs { get; set; }
    }
}
