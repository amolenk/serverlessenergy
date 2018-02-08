using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Documents;

namespace ServlessEnergyFunctionsApp
{
    public class ZeroDataForPeriodAlertInput
    {
        public Document Document { get; set; }

        public TimeSpan MonitorTimeSpan { get; set; }
    }
}
