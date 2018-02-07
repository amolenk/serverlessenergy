using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace ServlessEnergyFunctionsApp.Alerting
{
    public abstract class Alert
    {
        public abstract bool IsActive(Document document);
    }
}
