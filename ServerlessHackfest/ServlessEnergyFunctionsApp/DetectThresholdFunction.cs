using System;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace ServlessEnergyFunctionsApp
{
    public static class DetectThresholdFunction
    {
        private const string DeviceReadEventName = "DeviceRead";

        private static string key = TelemetryConfiguration.Active.InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
        private static TelemetryClient telemetry = new TelemetryClient() { InstrumentationKey = key };

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

                        var props = new Dictionary<string, string>();
                        props["DeviceId"] = deviceId;
                        props["ReadingChannel"] = readingChannel;

                        telemetry.TrackEvent("ThresholdExceeded", props);

                        log.LogWarning($"Threshold exceeded for device {deviceId}. Channel: {readingChannel} Value: {readingValue}");
                    }
                }
            }
        }

        private static bool ValueExceedsThreshold(decimal value)
        {
            // This threshold validation is fake.
            // The simulated meters that we use only push increasing values, therefore we need to to simulate the check if the reading exceeds a threshold.
            var divisibleByThree = Math.Floor(value) % 3 == 0;
            return divisibleByThree;
        }
    }
}
