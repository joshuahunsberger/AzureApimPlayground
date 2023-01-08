using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ApiManagement;
using Azure.ResourceManager.ApiManagement.Models;
using Microsoft.Extensions.Configuration;

var configManager = new ConfigurationManager()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json");

var configRoot = configManager
    .Build();

// Can authenticate with az login
var armClient = new ArmClient(new DefaultAzureCredential());

var azureSettings = new AzureSettings();
configRoot.Bind("AzureSettings", azureSettings);

var apimResourceIdBase = $"/subscriptions/{azureSettings.AzureSubscriptionId}/resourceGroups/{azureSettings.ApimResourceGroupName}/providers/Microsoft.ApiManagement/service/{azureSettings.ApimServiceName}";
var resourceId = new ResourceIdentifier(apimResourceIdBase);

var serviceResource = armClient.GetApiManagementServiceResource(resourceId);

var userCollection = serviceResource.GetApiManagementUsers();

var newUserEmail = configRoot["NewUserEmail"];
var existingUserResults = userCollection.GetAll($"email eq '{newUserEmail}'", top: 1);
var user = existingUserResults.FirstOrDefault();

if (user == null)
{
    var createContent = new ApiManagementUserCreateOrUpdateContent
    {
        Email = newUserEmail,
        FirstName = "Joshua",
        LastName = "Hunsberger",
    };

    var userOperation = await userCollection.CreateOrUpdateAsync(Azure.WaitUntil.Completed, Guid.NewGuid().ToString(), createContent, false);
    user = userOperation.Value;
}

if (user == null)
{
    Console.WriteLine("Unable to find or create user");
    Environment.Exit(-1);
}
