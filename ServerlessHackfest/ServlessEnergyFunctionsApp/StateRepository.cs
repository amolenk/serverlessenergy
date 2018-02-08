using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace ServlessEnergyFunctionsApp.Alerting
{
    public class VersionedState<T>
    {
        public string Version { get; set; }
        public T State { get; set; }
    }

    public class StateRepository
    {
        private readonly CloudBlobContainer _container;

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore
        };

        public StateRepository(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var client = storageAccount.CreateCloudBlobClient();

            _container = client.GetContainerReference("state");
        }

        public async Task<VersionedState<T>> LoadAsync<T>(string key)
        {
            var blob = _container.GetBlockBlobReference(key);

            if (await blob.ExistsAsync())
            {
                var json = await blob.DownloadTextAsync().ConfigureAwait(false);

                return new VersionedState<T>
                {
                    State = JsonConvert.DeserializeObject<T>(json, SerializerSettings),
                    Version = blob.Properties.ETag
                };
            }
            else
            {
                return new VersionedState<T>
                {
                    State = default(T),
                    Version = string.Empty
                };
            }
        }

        public Task SaveAsync<T>(string key, VersionedState<T> state)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(state, serializerSettings);
            var blob = _container.GetBlockBlobReference(key);

            var accessCondition = !string.IsNullOrWhiteSpace(state.Version)
                ? AccessCondition.GenerateIfMatchCondition(state.Version)
                : AccessCondition.GenerateIfNoneMatchCondition("*");

            return blob.UploadTextAsync(json, null, accessCondition, null, null);
        }
    }
}
