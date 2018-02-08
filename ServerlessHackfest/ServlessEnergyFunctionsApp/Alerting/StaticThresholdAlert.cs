using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;

namespace ServlessEnergyFunctionsApp.Alerting
{
    public class StaticThresholdAlert : Alert
    {
        public decimal? Min { get; set; }

        public decimal? Max { get; set; }

        public override bool IsActive(Document document)
        {
            var reading = document.GetPropertyValue<JObject>("reading");
            var readingValue = reading.Value<decimal>("value");

            return (Min.HasValue && readingValue < Min.Value)
                || (Max.HasValue && readingValue > Max.Value);
        }

        public override string ToString()
        {
            return $"Threshold(Min={Min}, Max={Max})";
        }
    }
}
