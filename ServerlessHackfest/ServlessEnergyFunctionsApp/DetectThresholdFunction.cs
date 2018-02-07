using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace ServlessEnergyFunctionsApp
{
    public static class DetectThresholdFunction
    {
        const string DeviceReadEventName = "DeviceRead";

        [FunctionName("DetectThresholdFunction")]
        public static void Run([CosmosDBTrigger("telemetrydb", "telemetry", ConnectionStringSetting = "CosmosDbServerlessamsterdamsqlConnectionString", CreateLeaseCollectionIfNotExists = true, LeaseCollectionName = "threshold-leases")] IReadOnlyList<Document> documents, ILogger log)
        {
            foreach (var document in documents)
            {
                var eventName = document.GetPropertyValue<string>("eventName");
                var deviceId = document.GetPropertyValue<string>("deviceId");

                if(DeviceReadEventName.Equals(eventName, StringComparison.OrdinalIgnoreCase))
                {
                    var reading = document.GetPropertyValue<JObject>("reading");
                    var readingChannel = reading.Value<string>("channelId");
                    var readingValue = reading.Value<decimal>("value");

                    var valueExceedsThreshold = ValueExceedsThreshold(readingValue);
                    if (valueExceedsThreshold)
                    {

                        log.LogWarning($"Threshold exceeded for device {deviceId}. Channel: {readingChannel} Value: {readingValue}");
                    }
                }
            }
        }

        private static bool ValueExceedsThreshold(decimal value)
        {
            // This threshold validation is fake. The simulated meters that we use only push increasing values.
            var divisibleByThree = Math.Floor(value) % 3 == 0;
            return divisibleByThree;
        }
    }
}
