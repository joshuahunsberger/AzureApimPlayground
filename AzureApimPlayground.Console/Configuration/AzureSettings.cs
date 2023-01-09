namespace AzureApimPlayground.Console.Configuration;

public class AzureSettings
{
    public string AzureSubscriptionId { get; set; }
    public string ApimResourceGroupName { get; set; }
    public string ApimServiceName { get; set; }
    public string ApimProductName { get; set; }

    public string ApimResourceId => $"/subscriptions/{AzureSubscriptionId}/resourceGroups/{ApimResourceGroupName}/providers/Microsoft.ApiManagement/service/{ApimServiceName}";

    public string ApimProductResourceId => $"{ApimResourceId}/products/{ApimProductName}";
}
