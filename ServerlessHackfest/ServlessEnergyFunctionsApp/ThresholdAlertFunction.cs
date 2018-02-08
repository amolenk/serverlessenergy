using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;

namespace ServlessEnergyFunctionsApp
{
    public static class ThresholdAlertFunction
    {
        [FunctionName("ThresholdAlertFunction")]
        public static bool Run(
            [ActivityTrigger] DurableActivityContext context,
            TraceWriter log)
        {
            var input = context.GetInput<ThresholdAlertInput>();

            var deviceId = input.Document.GetPropertyValue<string>("deviceId");

            var reading = input.Document.GetPropertyValue<JObject>("reading");
            var readingChannel = reading.Value<string>("channelId");
            var readingValue = reading.Value<decimal>("value");

            return (input.Min.HasValue && readingValue < input.Min.Value)
                || (input.Max.HasValue && readingValue > input.Max.Value);
         }
    }
}
