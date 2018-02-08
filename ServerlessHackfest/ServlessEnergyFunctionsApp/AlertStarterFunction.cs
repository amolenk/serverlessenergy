using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ServlessEnergyFunctionsApp
{
    public static class AlertStarterFunction
    {
        [FunctionName("AlertStarterFunction")]
        public static async Task Run(
            [CosmosDBTrigger("telemetrydb", "telemetry", ConnectionStringSetting = "CosmosDbServerlessamsterdamsqlConnectionString", CreateLeaseCollectionIfNotExists = true, LeaseCollectionName = "threshold-leases")] IReadOnlyList<Document> documents,
            [OrchestrationClient] DurableOrchestrationClient starter,
            TraceWriter log)
        {
            var orchestrationTasks = documents
                .Where(doc => doc.GetPropertyValue<string>("eventName") == "DeviceRead")
                .Select(doc => starter.StartNewAsync("AlertOrchestrationFunction", doc));

            await Task.WhenAll(orchestrationTasks);
        }
    }
}
