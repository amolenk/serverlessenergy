using System;
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

            // TODO Load configuration from storage to build the list of alerts dynamically.
            var alertTasks = new Task<bool>[]
            {
                //context.CallActivityAsync<bool>("ThresholdAlertFunction", new ThresholdAlertInput
                //{
                //    Document = document,
                //    Max = 100
                //}),
                context.CallActivityAsync<bool>("ZeroDataForPeriodAlertFunction", new ZeroDataForPeriodAlertInput
                {
                    Document = document,
                    MonitorTimeSpan = TimeSpan.FromMinutes(5)
                }),
            };

            //try
            //{
                // Get results from fan-out.
                var results = await Task.WhenAll(alertTasks);

                log.Warning($"GOT {results.Count(result => result)} ALERTS!");
            //}
            //catch (Exception ex)
            //{
            //    log.Error(ex.Message);
            //}

        }
    }
}