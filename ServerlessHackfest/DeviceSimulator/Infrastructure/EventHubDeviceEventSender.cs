using DeviceSimulator.Events;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator.Infrastructure
{
    public class EventHubDeviceEventSender
    {
        private readonly JsonSerializer _serializer;
        private readonly EventHubClient _client;

        public EventHubDeviceEventSender(string eventhubConnectionString)
        {
            _serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            _client = EventHubClient.CreateFromConnectionString(eventhubConnectionString);
        }

        public Task SendAsync(DeviceEventBase @event, CancellationToken cancellationToken)
        {
            var eventData = GetEventData(@event);
            return _client.SendAsync(eventData, @event.DeviceId);
        }

        private EventData GetEventData(DeviceEventBase @event)
        {
            var evt = JObject.FromObject(@event, _serializer);
            evt["eventName"] = @event.GetType().Name;

            var payload = Encoding.UTF8.GetBytes(evt.ToString(Formatting.None));

            var eventData = new EventData(payload);
            eventData.Properties["eventName"] = @event.GetType().Name;

            foreach (var property in evt["properties"].Children<JProperty>())
            {
                eventData.Properties[property.Name] = property.Value.ToString();
            }

            return eventData;
        }
    }
}
