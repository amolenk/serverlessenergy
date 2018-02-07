using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using Microsoft.Azure.Documents;

namespace ServlessEnergyFunctionsApp
{
    public static class DetectThresholdFunction
    {
        [FunctionName("DetectThresholdFunction")]
        public static void Run([CosmosDBTrigger("telemetrydb", "telemetry", ConnectionStringSetting = "CosmosDbServerlessamsterdamsqlConnectionString", CreateLeaseCollectionIfNotExists = true, LeaseCollectionName = "threshold-leases")] IReadOnlyList<Document> documents, TraceWriter log)
        {
            foreach (var document in documents)
            {
                var eventName = document.GetPropertyValue<string>("eventName");
                var deviceId = document.GetPropertyValue<string>("deviceId");

                log.Info($"Receiveid document {eventName} for device {deviceId}");

            }
        }
    }
}
