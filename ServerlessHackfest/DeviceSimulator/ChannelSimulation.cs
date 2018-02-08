using DeviceSimulator.Events;
using DeviceSimulator.Infrastructure;
using DeviceSimulator.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator
{
    public class ChannelSimulation
    {
        private readonly string _deviceId;
        private readonly string _channel;
        private readonly string _unit;
        private readonly EventHubDeviceEventSender _sender;

        public ChannelSimulation(string deviceId, string channel, string unit, EventHubDeviceEventSender sender)
        {
            _deviceId = deviceId;
            _channel = channel;
            _unit = unit;

            _sender = sender;
        }

        public Task RunAsync(SimulationStrategy strategy, CancellationToken cancellationToken)
        {
            return strategy.RunAsync((value) => {
                var reading = new DeviceReading(DateTime.UtcNow, value, _channel, string.Empty, _unit);

                var eventProps = new Dictionary<string, string>();
                eventProps["ProjectId"] = "Foo";

                var @event = new DeviceRead(_deviceId, reading, "console-sim", string.Empty, eventProps, DateTime.UtcNow);

                _sender.SendAsync(@event, cancellationToken);
            }, cancellationToken);
        }
    }
}
