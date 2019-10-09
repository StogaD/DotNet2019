using CoreWebApp.Configuration;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApp.KeyVault
{



    public interface ISecretAccess
    {
        Task SetSecret(string secretValue);
        Task<string> GetSecret(string secretName);
    }

    public class SecretAccess : ISecretAccess
    {
        private readonly KeyVaultOptions _keyVaultOptions;
        private readonly string _kvURL;
        public SecretAccess(IOptions<KeyVaultOptions> keyVaultOptions)
        {
            _keyVaultOptions = keyVaultOptions.Value;
          
            _kvURL = $"https://{_keyVaultOptions.KeyVaultName}.vault.azure.net";
        }

        public async Task<string> GetSecret(string inputName)
        {
           var secretName = string.IsNullOrWhiteSpace(inputName) ? _keyVaultOptions.SecretName : inputName;
           var kvClient = this.CreateClinet();
           var keyVaultSecret = await kvClient.GetSecretAsync(_kvURL, secretName).ConfigureAwait(false);

            return keyVaultSecret.Value;

        }
        public async Task SetSecret(string secretValue)
        {
            var kvClient = this.CreateClinet();
            await kvClient.SetSecretAsync(_kvURL, _keyVaultOptions.SecretName, $"{secretValue}_{DateTime.Now.ToString()}");

        }

        public KeyVaultClient CreateClinet()
        {
            // <authentication>

            // setx - command to set environment. 
            string clientId = Environment.GetEnvironmentVariable("akvClientId");
            string clientSecret = Environment.GetEnvironmentVariable("akvClientSecret");

            var kvClient = new KeyVaultClient(async (authority, resource, scope) =>
                {
                    var adCredential = new ClientCredential(clientId, clientSecret);
                    var authenticationContext = new AuthenticationContext(authority, null);
                    return (await authenticationContext.AcquireTokenAsync(resource, adCredential)).AccessToken;
                });
            return kvClient;
        }


    }
}
