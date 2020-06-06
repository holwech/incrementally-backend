using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace incrementally.Services
{
    public class CosmosDbService : IDatabaseConnector
    {
        private Dictionary<string, Container> _containers;
        private CosmosClient _client;
        private DatabaseResponse _database;
        private string _databaseName;

        public CosmosDbService(string databaseName)
        {
            _databaseName = databaseName;
            _containers = new Dictionary<string, Container>();
        }

        public async Task InitializeAsync(string databaseName, List<string> containerNames, string account, string key)
        {
            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(account, key);
            _client = clientBuilder
              .WithConnectionModeDirect()
              .Build();
            _database = await _client.CreateDatabaseIfNotExistsAsync(databaseName);
            containerNames.ForEach(async containerName => await CreateContainer(containerName));
        }

        public async Task CreateContainer(string containerName)
        {
            _containers.Add(containerName, _client.GetContainer(_databaseName, containerName));
            await _database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
        }

        public async Task AddRecordingAsync(RecordingMetadata recordingMetadata)
        {
            await _containers["recordings"].CreateItemAsync(recordingMetadata, new PartitionKey(recordingMetadata.Id));
        }

        public async Task<IEnumerable<RecordingMetadata>> GetTopRecordings()
        {
            var query = new QueryDefinition("SELECT * FROM c ORDER BY c.createdAt DESC OFFSET 0 LIMIT 10");
            var resultSetIterator = _containers["recordings"].GetItemQueryIterator<RecordingMetadata>(query);
            var results = new List<RecordingMetadata>();
            while (resultSetIterator.HasMoreResults)
            {
                results.AddRange(await resultSetIterator.ReadNextAsync());
            }
            return results;
        }

        public async Task<RecordingMetadata> GetRecording(string id)
        {
            return await _containers["recordings"].ReadItemAsync<RecordingMetadata>(id, new PartitionKey(id));
        }
    }
}
