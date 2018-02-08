//using System;
//using Microsoft.Azure.WebJobs;
//using System.Collections.Generic;
//using Microsoft.Azure.Documents;
//using Newtonsoft.Json.Linq;
//using Microsoft.ApplicationInsights.Extensibility;
//using Microsoft.ApplicationInsights;
//using Microsoft.Extensions.Logging;
//using ServlessEnergyFunctionsApp.Alerting;
//using System.Threading.Tasks;
//using System.Linq;

//namespace ServlessEnergyFunctionsApp
//{
//    public static class DetectThresholdFunction
//    {
//        private const string DeviceReadEventName = "DeviceRead";

//        private static readonly IAlertConfigurationRepository AlertConfigurationRepo = new BlobAlertConfigurationRepository(Environment.GetEnvironmentVariable("ConfigStorage"));

//        private static TelemetryClient TelemetryClient;

//        static DetectThresholdFunction()
//        {
//            TelemetryConfiguration.Active.InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
//            TelemetryClient = new TelemetryClient();
//        }

//        [FunctionName("DetectThresholdFunction")]
//        public static async Task Run([CosmosDBTrigger("telemetrydb", "telemetry", ConnectionStringSetting = "CosmosDbServerlessamsterdamsqlConnectionString", CreateLeaseCollectionIfNotExists = true, LeaseCollectionName = "threshold-leases")] IReadOnlyList<Document> documents, ILogger log)
//        {
//            foreach (var document in documents)
//            {
//                var eventName = document.GetPropertyValue<string>("eventName");
//                var deviceId = document.GetPropertyValue<string>("deviceId");

//                if(DeviceReadEventName.Equals(eventName, StringComparison.OrdinalIgnoreCase))
//                {
//                    var reading = document.GetPropertyValue<JObject>("reading");
//                    var readingChannel = reading.Value<string>("channelId");

//                    // TODO Get projectId from event instead of hard coded "Foo"!
//                    var config = await AlertConfigurationRepo.GetAlertConfigurationAsync("Foo");

//                    var activeAlerts = config.Alerts.Where(alert => alert.IsActive(document));

//                    foreach (var activeAlert in activeAlerts)
//                    {
//                        var props = new Dictionary<string, string>();
//                        props["DeviceId"] = deviceId;
//                        props["ReadingChannel"] = readingChannel;

//                        TelemetryClient.TrackEvent(activeAlert.ToString(), props);

//                        log.LogWarning($"Alert activated for device {deviceId}. Channel: {readingChannel} Alert: {activeAlert}");
//                    }
//                }
//            }
//        }
//    }
//}
