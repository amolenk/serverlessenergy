using System.Threading.Tasks;

namespace ServlessEnergyFunctionsApp.Alerting
{
    public interface IAlertConfigurationRepository
    {
        Task<AlertConfiguration> GetAlertConfigurationAsync(string projectId);

        Task SetAlertConfigurationAsync(AlertConfiguration configuration);
    }
}
