using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ServlessEnergyFunctionsApp
{
    public static class AlertOrchestrationFunction
    {
        [FunctionName("AlertOrchestrationFunction")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            TraceWriter log)
        {
            var document = context.GetInput<Document>();
            var alertResults = await RunAlerts(context, document);

            var triggeredAlerts = alertResults.Where(x => x.Triggered);

            foreach (var alert in triggeredAlerts)
            {
                log.Warning($"Triggered alert {alert.AlertName}: {alert.AlertMessage}");
            }
        }

        private static async Task<List<AlertResult>> RunAlerts(DurableOrchestrationContext context, Document document)
        {
            // TODO Load configuration from storage to build the list of alerts dynamically.
            var alertResults = new List<AlertResult>();

            await RunThresholdAlert(context, document, alertResults);
            await RunZeroDataForPeriodAlert(context, document, alertResults);

            return alertResults;
        }

        private static async Task RunZeroDataForPeriodAlert(DurableOrchestrationContext context, Document document, List<AlertResult> alertResults)
        {
            var zeroDataForPeriodAlertResult = await context.CallActivityAsync<AlertResult>("ZeroDataForPeriodAlertFunction", new ZeroDataForPeriodAlertInput
            {
                Document = document,
                MonitorTimeSpan = TimeSpan.FromSeconds(10),
            });

            alertResults.Add(zeroDataForPeriodAlertResult);
        }

        private static async Task RunThresholdAlert(DurableOrchestrationContext context, Document document, List<AlertResult> alertResults)
        {
            var thresholdAlertResult = await context.CallActivityAsync<AlertResult>("ThresholdAlertFunction", new ThresholdAlertInput
            {
                Document = document,
                Min = 228,
                Max = 232,
            });

            alertResults.Add(thresholdAlertResult);
        }
    }
}