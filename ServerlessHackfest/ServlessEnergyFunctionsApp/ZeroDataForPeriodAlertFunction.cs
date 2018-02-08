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
        public static async Task<AlertResult> Run(
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
            var versionedState = await UpdateState(log, deviceId, value, timestamp, stateKey);

            var rv = new AlertResult()
            {
                AlertName = "ZeroDataForPeriodAlert",
                Triggered = false,
            };

            if (versionedState.State.ZeroSince.HasValue && DateTime.UtcNow - versionedState.State.ZeroSince.Value > input.MonitorTimeSpan)
            {
                rv.AlertMessage = $"Device with id {deviceId} is publishing 0 values since {versionedState.State.ZeroSince.Value} while a maximum timespan of {input.MonitorTimeSpan} is allowed";
                rv.Triggered = true;
            }

            return rv;
        }

        private static async Task<VersionedState<ZeroDataForPeriodAlertState>> UpdateState(TraceWriter log, string deviceId, decimal value, DateTime timestamp, string stateKey)
        {
            var versionedState = await Repository.LoadAsync<ZeroDataForPeriodAlertState>(stateKey);

            bool succes = false;
            bool dirty = false;

            do
            {
                try
                {
                    if (versionedState.State == null)
                    {
                        versionedState.State = new ZeroDataForPeriodAlertState() { DeviceId = deviceId };
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
            return versionedState;
        }
    }

    public class ZeroDataForPeriodAlertState
    {
        public string DeviceId { get; set; }

        public DateTime? ZeroSince { get; set; }
    }
}
