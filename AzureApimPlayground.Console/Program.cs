using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ApiManagement;
using Azure.ResourceManager.ApiManagement.Models;

// Can authenticate with az login
var armClient = new ArmClient(new DefaultAzureCredential());

// TODO: Add resource ID for APIM subscription
var resourceId = new ResourceIdentifier("");

var serviceResource = armClient.GetApiManagementServiceResource(resourceId);

var userCollection = serviceResource.GetApiManagementUsers();

// TODO: Add desired email
const string newUserEmail = "";
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
