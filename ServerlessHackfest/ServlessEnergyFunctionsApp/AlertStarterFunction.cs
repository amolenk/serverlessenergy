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
            //            log.Warning($"GOT A COSMOS BATCH: {documents.Count}");

            var orchestrationTasks = documents
                .Where(doc => doc.GetPropertyValue<string>("eventName") == "DeviceRead")
                .Select(doc => starter.StartNewAsync("AlertOrchestrationFunction", doc));
                //.Select(async doc =>
                //{
                //    var id = await starter.StartNewAsync("AlertOrchestrationFunction", doc);

                //    log.Warning($"STARTING NEW ORCHESTRATION WITH ID {id}");
                //});

            await Task.WhenAll(orchestrationTasks);
        }
    }
}
