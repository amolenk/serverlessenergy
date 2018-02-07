using System.Collections.Generic;

namespace ServlessEnergyFunctionsApp.Alerting
{
    public class AlertConfiguration
    {
        public string ProjectId { get; set; }

        public List<Alert> Alerts { get; set; }
    }
}
