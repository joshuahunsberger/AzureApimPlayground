# Azure APIM Playground

This project demonstrates how to interact with Azure Resource Manager via the [Azure .NET SDK](https://www.nuget.org/packages/Azure.ResourceManager.ApiManagement/1.0.0) to create users and subscriptions in [Azure API Management](https://learn.microsoft.com/en-us/azure/api-management/).

## Setup

### Configure an APIM Service

You can create an Azure API Management instance by following the tutorial. https://learn.microsoft.com/en-us/azure/api-management/get-started-create-service-instance

*Note*: Provisioning an APIM service resource can take 30+ minutes.

Setup an API:
https://learn.microsoft.com/en-us/azure/api-management/import-and-publish

Create a Product: https://learn.microsoft.com/en-us/azure/api-management/api-management-howto-add-products?tabs=azure-portal

### App Configuration

You can use the names in `appsettings.json`, but if you use different values in the tutorial steps, replace the configured values.

Certain values are secret enough that you may not want them in source control. The console project is setup with [dotnet user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows).

For instance, you can set your AzureSubscriptionId when managing `secrets.json` through Visual Studio:
```
{
    "AzureSettings": {
        "AzureSubscriptionId": "REPLACE-ME"
    }
}
```

