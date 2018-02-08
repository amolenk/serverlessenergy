using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DeviceSimulator.Model
{
    [DataContract]
    public class DeviceReading
    {
        public DeviceReading(
            DateTime timestamp,
            decimal value,
            string channelId,
            string description,
            string unit)
            : this(timestamp, value, channelId, description, unit, new Dictionary<string, string>())
        {
        }

        public DeviceReading(
            DateTime timestamp,
            decimal value,
            string channelId,
            string description,
            string unit,
            IDictionary<string, string> properties)
        {
            Timestamp = timestamp;
            Value = value;
            ChannelId = channelId;
            Description = description;
            Unit = unit;
            Properties = properties;
        }

        [JsonConstructor]
        private DeviceReading()
        {
        }

        [JsonProperty]
        public virtual DateTime Timestamp { get; private set; }

        [JsonProperty]
        public virtual decimal Value { get; private set; }

        [JsonProperty]
        public virtual string ChannelId { get; private set; }

        [JsonProperty]
        public virtual string Description { get; private set; }

        [JsonProperty]
        public virtual string Unit { get; private set; }

        [JsonProperty]
        public IDictionary<string, string> Properties { get; private set; }

        public override string ToString()
        {
            var dictionaryString = string.Join(", ", Properties
                .Select(kv => string.Format("{0} = {1}", kv.Key, kv.Value)));

            string baseChannelId = string.Empty;

            return $"[Channel = {ChannelId}{baseChannelId}] {Timestamp}: {Value} {Unit} [Description = {Description}, {dictionaryString}]";
        }
    }
}
