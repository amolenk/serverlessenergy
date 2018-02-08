using DeviceSimulator.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator
{
    class Program
    {
        private const string DevicesWithPeakStrategyStateFileName = "deviceswithpeakstrat.state";

        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables("DeviceSimulator:");

            Configuration = builder.Build();

            using (var cts = new CancellationTokenSource())
            {
                var simulationTasks = new List<Task>();

                var eventhubConnectionString = Configuration["EventHubConnectionString"];
                var eventhubEventSender = new EventHubDeviceEventSender(eventhubConnectionString);

                int numberOfDevicesWithPeakStrategy = int.Parse(Configuration["NumberOfDevicesWithPeakStrategy"]);
                var devicesWithPeakStrategy = SetupDevicesWithPeakStrategy(eventhubEventSender, simulationTasks, numberOfDevicesWithPeakStrategy, cts.Token);

                simulationTasks.AddRange(devicesWithPeakStrategy);

                Console.ReadKey(true);
                cts.Cancel();

                Console.WriteLine("Quitting. Waiting for all simulations tasks to finish.");
                Task.WhenAll(simulationTasks).GetAwaiter().GetResult();
            }
            
        }

        private static IEnumerable<Task> SetupDevicesWithPeakStrategy(EventHubDeviceEventSender eventhubEventSender, List<Task> simulationTasks, int numberOfDevicesWithPeakStrategy, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Setting up {numberOfDevicesWithPeakStrategy} devices with a peaking strategy");

            var devicesWithPeakStrat = LoadDevices(DevicesWithPeakStrategyStateFileName, numberOfDevicesWithPeakStrategy);

            var peakIntervals = new int[] { 5, 10, 15, 20 };
            var random = new Random();

            foreach (var device in devicesWithPeakStrat)
            {
                // Randomizing the peak interval for some variation between devices
                var peakIntervalSeconds = random.Next(0, peakIntervals.Length);

                var sim = new ChannelSimulation(device, "register://electricity/0/voltage/sumli", "V", eventhubEventSender);
                var strat = new PeakStrategy()
                {
                    MinValue = 228,
                    MaxValue = 232,
                    PeakValue = 300,
                    ValueInterval = TimeSpan.FromMilliseconds(100),
                    PeakInterval = TimeSpan.FromSeconds(peakIntervalSeconds)
                };

                var simTask = sim.RunAsync(strat, cancellationToken);
                yield return simTask;
            }
        }

        private static List<string> LoadDevices(string fileName, int numberOfDevices)
        {
            List<string> devices = new List<string>();

            using (var file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var writer = new StreamWriter(file);

                var reader = new StreamReader(file);
                var device = reader.ReadLine();

                while (device != null)
                {
                    devices.Add(device);
                    device = reader.ReadLine();
                }

                if (devices.Count < numberOfDevices)
                {
                    for (int i = 0; i < numberOfDevices - devices.Count; i++)
                    {
                        var newDeviceId = Guid.NewGuid().ToString();
                        devices.Add(newDeviceId);
                        writer.WriteLine(newDeviceId);
                    }

                    writer.Flush();
                }
            }

            return devices;
        }
    }
}
