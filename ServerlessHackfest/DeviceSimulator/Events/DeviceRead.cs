using System;
using System.Collections.Generic;
using DeviceSimulator.Model;
using Newtonsoft.Json;

namespace DeviceSimulator.Events
{
    public class DeviceRead : DeviceEventBase
    {
        public DeviceRead(
            string deviceId,
            DeviceReading reading,
            string protocolType,
            string connectivityZone,
            Dictionary<string, string> properties,
            DateTime? eventTimestamp = null)
            : base(deviceId, properties, eventTimestamp)
        {
            ProtocolType = protocolType;
            ConnectivityZone = connectivityZone;
            Reading = reading;
        }

        [JsonConstructor]
        private DeviceRead()
        {
        }

        [JsonProperty]
        public DeviceReading Reading { get; private set; }

        [JsonProperty]
        public string ProtocolType { get; private set; }

        [JsonProperty]
        public string ConnectivityZone { get; private set; }
    }
}
