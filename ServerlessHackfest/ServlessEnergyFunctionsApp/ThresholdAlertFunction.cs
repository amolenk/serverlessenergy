using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;

namespace ServlessEnergyFunctionsApp
{
    public static class ThresholdAlertFunction
    {
        [FunctionName("ThresholdAlertFunction")]
        public static AlertResult Run(
            [ActivityTrigger] DurableActivityContext context,
            TraceWriter log)
        {
            var input = context.GetInput<ThresholdAlertInput>();

            var deviceId = input.Document.GetPropertyValue<string>("deviceId");

            var reading = input.Document.GetPropertyValue<JObject>("reading");
            var readingChannel = reading.Value<string>("channelId");
            var readingValue = reading.Value<decimal>("value");

            var result = (input.Min.HasValue && readingValue < input.Min.Value)
                || (input.Max.HasValue && readingValue > input.Max.Value);

            return  new AlertResult()
            {
                AlertName = "ThresholdExceededAlert",
                AlertMessage = $"Device with id {deviceId} exceeded threshold boundary of min {input.Min} and max {input.Max}. Value: {readingValue}",
                Triggered = result,
            };
         }
    }
}
