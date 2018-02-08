using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace ServlessEnergyFunctionsApp.Alerting
{
    public class BlobAlertConfigurationRepository : IAlertConfigurationRepository
    {
        private readonly CloudBlobContainer _container;

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore
        };

        public BlobAlertConfigurationRepository(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var client = storageAccount.CreateCloudBlobClient();

            _container = client.GetContainerReference("config");
            _container.CreateIfNotExistsAsync().GetAwaiter().GetResult(); // Not cool in ctor
        }

        public async Task<AlertConfiguration> GetAlertConfigurationAsync(string projectId)
        {
            var blob = _container.GetBlockBlobReference(projectId);
            var json = await blob.DownloadTextAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<AlertConfiguration>(json, SerializerSettings);
        }

        public Task SetAlertConfigurationAsync(AlertConfiguration configuration)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(configuration, serializerSettings);

            System.Console.WriteLine(json);

            var blob = _container.GetBlockBlobReference(configuration.ProjectId);

            return blob.UploadTextAsync(json);
        }
    }
}
