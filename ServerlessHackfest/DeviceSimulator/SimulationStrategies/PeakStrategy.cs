using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator
{
    public class PeakStrategy : SimulationStrategy
    {
        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public int PeakValue { get; set; }

        public TimeSpan PeakInterval { get; set; }

        public TimeSpan ValueInterval { get; set; }

        public override async Task RunAsync(Action<decimal> onNewValue, CancellationToken cancellationToken)
        {
            var random = new Random();
            var runtime = TimeSpan.Zero;

            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextValue = random.Next(MinValue, MaxValue);
                    var shouldPeak = runtime > PeakInterval;

                    if (shouldPeak)
                    {
                        nextValue = PeakValue;
                        runtime = TimeSpan.Zero;
                    }

                    onNewValue(nextValue);

                    await Task.Delay(ValueInterval, cancellationToken);
                    runtime = runtime.Add(ValueInterval);
                }
            }
            catch (OperationCanceledException)
            {
            }
            
        }

    }
}
