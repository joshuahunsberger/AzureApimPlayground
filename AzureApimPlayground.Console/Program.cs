using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ApiManagement;
using Azure.ResourceManager.ApiManagement.Models;
using AzureApimPlayground.Console;
using Microsoft.Extensions.Configuration;

var configManager = new ConfigurationManager()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>();

var configRoot = configManager
    .Build();

// Can authenticate with az login
var armClient = new ArmClient(new DefaultAzureCredential());

var azureSettings = new AzureSettings();
configRoot.Bind("AzureSettings", azureSettings);

var resourceId = new ResourceIdentifier(azureSettings.ApimResourceId);

var serviceResource = armClient.GetApiManagementServiceResource(resourceId);

var userCollection = serviceResource.GetApiManagementUsers();

var newUserSettings = new NewUserSettings();
configRoot.Bind("NewUserSettings", newUserSettings);
var newUserEmail = configRoot["NewUserEmail"];
var existingUserResults = userCollection.GetAll($"email eq '{newUserEmail}'", top: 1);
var user = existingUserResults.FirstOrDefault();

if (user == null)
{
    var createContent = new ApiManagementUserCreateOrUpdateContent
    {
        Email = newUserSettings.Email,
        FirstName = newUserSettings.FirstName,
        LastName = newUserSettings.LastName,
        Note = "Created by .NET SDK"
    };

    var userOperation = await userCollection.CreateOrUpdateAsync(Azure.WaitUntil.Completed, Guid.NewGuid().ToString(), createContent, false);
    user = userOperation.Value;
}

if (user == null)
{
    Console.WriteLine("Unable to find or create user");
    Environment.Exit(-1);
}

// Check for existing subscription
var userSubscriptionCollection = user.GetApiManagementUserSubscriptions();

var existingSubscriptionPages = userSubscriptionCollection.GetAll();
var userSubscriptions = existingSubscriptionPages.ToList();

var existingSubscription = userSubscriptions
    .Where(us => us.Data.Scope.Equals(azureSettings.ApimProductResourceId, StringComparison.InvariantCultureIgnoreCase))
    .FirstOrDefault();

// Create subscription
if (existingSubscription == null)
{
    var subscriptionCollection = serviceResource.GetApiManagementSubscriptions();
    var subscriptionCreateContent = new ApiManagementSubscriptionCreateOrUpdateContent
    {
        OwnerId = user.Id,
        Scope = azureSettings.ApimProductResourceId,
        State = SubscriptionState.Active,
        DisplayName = ".NET SDK Created Product Demo Conference API subscription"
    };
    var subscription = await subscriptionCollection.CreateOrUpdateAsync(Azure.WaitUntil.Completed, Guid.NewGuid().ToString(), subscriptionCreateContent, true);
    Console.WriteLine($"Keys:\n\t{subscription.Value.Data.PrimaryKey}\n\t{subscription.Value.Data.SecondaryKey}");
}
else
{
    Console.WriteLine("Subscription already exists.");
}
