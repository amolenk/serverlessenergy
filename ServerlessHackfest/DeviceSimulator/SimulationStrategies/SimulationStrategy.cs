using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator
{
    public abstract class SimulationStrategy
    {
        public abstract Task RunAsync(Action<decimal> onNewValue, CancellationToken cancellationToken);
    }
}