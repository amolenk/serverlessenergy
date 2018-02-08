using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Documents;

namespace ServlessEnergyFunctionsApp
{
    public class ThresholdAlertInput
    {
        public Document Document { get; set; }

        public decimal? Min { get; set; }

        public decimal? Max { get; set; }
    }
}
