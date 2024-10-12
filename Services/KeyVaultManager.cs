using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace PhotocopyRevaluationAppMVC.Services
{
    public static class KeyVaultManager
    {
        private static readonly SecretClient _secretClient;

        static KeyVaultManager()
        {
            // Replace with your Key Vault URI
            string keyVaultUrl = "https://<your-keyvault-name>.vault.azure.net/";

            // DefaultAzureCredential will use environment settings, managed identities, etc.
            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }

        public static async Task<string> GetKeyVaultSecretKey(string Key) 
        {
            KeyVaultSecret secret = _secretClient.GetSecret(Key);
            return secret.Value;
        }
        public static async Task StoreSecretToKeyVaultAsync(string secretName, string secretValue)
        {
            // Store the secret in Key Vault
            KeyVaultSecret secret = new KeyVaultSecret(secretName, secretValue);

            // Setting optional parameters like expiry, contentType
            secret.Properties.ExpiresOn = DateTimeOffset.UtcNow.AddYears(1);
            secret.Properties.ContentType = "text/plain";

            // Add the secret to Key Vault
            await _secretClient.SetSecretAsync(secret);

            Console.WriteLine($"Secret '{secretName}' stored successfully.");
        }
    }
}
