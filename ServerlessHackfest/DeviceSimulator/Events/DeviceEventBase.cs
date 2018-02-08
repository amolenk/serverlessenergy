using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DeviceSimulator.Events
{
    public abstract class DeviceEventBase
    {
        protected DeviceEventBase()
            : this(Guid.NewGuid().ToString(), new Dictionary<string, string>(), DateTime.UtcNow)
        {
        }

        protected DeviceEventBase(string deviceId, Dictionary<string, string> properties, DateTime? timestamp = null)
        {
            DeviceId = deviceId;
            Properties = properties;
            Timestamp = timestamp.HasValue ? timestamp.Value : DateTime.UtcNow;
        }

        [JsonProperty]
        public string DeviceId { get; private set; }

        [JsonProperty]
        public Dictionary<string, string> Properties { get; private set; }

        [JsonProperty]
        public DateTime Timestamp { get; private set; }
    }
}
