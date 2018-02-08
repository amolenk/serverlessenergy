using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json.Linq;
using ServlessEnergyFunctionsApp.Alerting;

namespace ServlessEnergyFunctionsApp
{
    public static class ZeroDataForPeriodAlertFunction
    {
        private static readonly StateRepository Repository = new StateRepository(Environment.GetEnvironmentVariable("StateStorage"));
 
        [FunctionName("ZeroDataForPeriodAlertFunction")]
        public static async Task<bool> Run(
            [ActivityTrigger] DurableActivityContext context,
            TraceWriter log)
        {
            var input = context.GetInput<ZeroDataForPeriodAlertInput>();

            var deviceId = input.Document.GetPropertyValue<string>("deviceId");
            var reading = input.Document.GetPropertyValue<JObject>("reading");
            var channelId = reading["channelId"].Value<string>();
            var value = reading["value"].Value<decimal>();
            var timestamp = reading["timestamp"].Value<DateTime>();

            var stateKey = $"{deviceId}_{channelId}".Replace(':', '_').Replace('/', '_');

            bool succes;
            bool dirty = false;
            do
            {
                try
                {
                    var versionedState = await Repository.LoadAsync<ZeroDataForPeriodAlertState>(stateKey);

                    if (versionedState.State == null)
                    {
                        versionedState.State = new ZeroDataForPeriodAlertState();
                        dirty = true;
                    }

                    if (value == 0 && !versionedState.State.ZeroSince.HasValue)
                    {
                        versionedState.State.ZeroSince = timestamp;
                        dirty = true;
                    }
                    else if (value > 0 && versionedState.State.ZeroSince.HasValue)
                    {
                        versionedState.State.ZeroSince = null;
                        dirty = true;
                    }

                    // TODO Check if we should raise an alert.


                    if (dirty)
                    {
                        await Repository.SaveAsync(stateKey, versionedState);
                    }

                    succes = true;
                }
                catch (StorageException ex)
                {
                    log.Warning(ex.Message);
                    succes = false;
                }
            }
            while (!succes);

            return false;
        }
    }

    public class ZeroDataForPeriodAlertState
    {
        public DateTime? ZeroSince { get; set; }
    }
}
