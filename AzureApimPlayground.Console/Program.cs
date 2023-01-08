using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ApiManagement;
using Azure.ResourceManager.ApiManagement.Models;
using Microsoft.Extensions.Configuration;

var configManager = new ConfigurationManager()
    .AddUserSecrets<Program>();

var configRoot = configManager
    .Build();

// Can authenticate with az login
var armClient = new ArmClient(new DefaultAzureCredential());

var subscriptionId = configRoot["AzureSubscriptionId"];
var apimResourceIdString = $"/subscriptions/{subscriptionId}/resourceGroups/apim-playground/providers/Microsoft.ApiManagement/service/apim-playground-hunsberger";
var resourceId = new ResourceIdentifier(apimResourceIdString);

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
