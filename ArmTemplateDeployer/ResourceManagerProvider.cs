using System;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace ArmTemplateDeployer
{
    public class ResourceManagerProvider
    {
        private const string AuthenticationAuthority = "https://login.windows.net/{0}";
        private const string ResourceProviderUri = "https://management.core.windows.net/";
        private const string ApiVersion = "2015-08-15-preview";

        private readonly string clientId;
        private readonly string clientSecret;

        public ResourceManagerProvider(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        private string GetAccessToken(string tenantId)
        {
            var authenticationContext = new AuthenticationContext(string.Format(AuthenticationAuthority, tenantId));
            var credential = new ClientCredential(clientId: clientId, clientSecret: clientSecret);
            var result = authenticationContext.AcquireTokenAsync(resource: ResourceProviderUri, clientCredential: credential).Result;
            
            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }
            return result.AccessToken;
        }

        public IResourceManagementClient GetResourceManagementClient(string subscriptionId, string tenantId)
        {
            var accessToken = GetAccessToken(tenantId);
            var tokenCredentials = new TokenCredentials(accessToken);
            var client = new ResourceManagementClient(tokenCredentials);
            client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            client.SubscriptionId = subscriptionId.ToString();
            return client;
        }
    }
}
