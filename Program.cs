using Azure.Identity;
using Microsoft.Identity.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using Smartersoft.Identity.Client.Assertion;

public class TokenProvider
{


    public async Task<string> GetToken (CancellationToken cancellationToken)
    {
        // Create a token credential that suits your needs, used to access the KeyVault
        Console.WriteLine("Get DefaultAzureCredential");
        var tokenCredential = new DefaultAzureCredential();

        const string clientId = "70e9ae2c-d00b-4581-9a31-b71c2a0ec38e";
        const string tenantId = "ffb9e08b-6f50-443c-8fe8-48aed2dc3204";
        const string KeyVaultUri = "https://sundmankv1.vault.azure.net/";
        const string certificateName = "cert2";

        Uri? keyId = null;
        string? kid = null;

        // Load once and save in Cache/Config/...
        Console.WriteLine("Get KeyVaulyKey");
        // var certificateInfo = await ClientAssertionGenerator.GetCertificateInfoFromKeyVault(new Uri(KeyVaultUri), certificateName, tokenCredential, cancellationToken);
        // if (certificateInfo.Kid == null || certificateInfo.KeyId == null)
        // {
        //     throw new Exception();
        // }
        // keyId = certificateInfo.KeyId;
        // kid = certificateInfo.Kid;

        keyId = new Uri ("https://sundmankv1.vault.azure.net/keys/cert2/0b750f4673d0434eb1aaf7f907d1e13b");
        kid = "GC9hy9fTNW8BaCCBqPPlPSWAcKM";

        Console.WriteLine($"keyId: {keyId}");
        Console.WriteLine($"kid: {kid}");

        // Use the ConfidentialClientApplicationBuilder as usual
        // but call `.WithKeyVaultCertificate(...)` instead of `.WithCertificate(...)`

        Console.WriteLine("Get Token from KeyVaultCertificate");
        var app = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
            // .WithKeyVaultCertificate(new Uri(KeyVaultUri), certificateName, tokenCredential)
            .WithKeyVaultKey(keyId, kid, tokenCredential)
            .Build();

        // Use the app, just like before
        Console.WriteLine("Acquire Token for Graph API using the KeyVaultCertificate");
        var tokenResult = await app.AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" })
            .ExecuteAsync(cancellationToken);

        return tokenResult.AccessToken;
    }

    // Entry point of the application
    public static async Task Main(string[] args)
    {
        try
        {
            var tokenProvider = new TokenProvider();
            string accessToken = await tokenProvider.GetToken(default);

            Console.WriteLine($"Access Token: {accessToken}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

