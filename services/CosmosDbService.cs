namespace incrementally.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.Azure.Cosmos;
  using models.recording;

  public class CosmosDbService : ICosmosDbService
    {
      private Container _container;

      public CosmosDbService(
          CosmosClient dbClient,
          string databaseName,
          string containerName)
      {
          this._container = dbClient.GetContainer(databaseName, containerName);
      }
      
      public async Task AddItemAsync(RecordingEntry item)
      {
          await this._container.CreateItemAsync<RecordingEntry>(item, new PartitionKey(item.Id));
      }

      public async Task<RecordingEntry> GetItemAsync(string id)
      {
          ItemResponse<RecordingEntry> response = await this._container.ReadItemAsync<RecordingEntry>(id, new PartitionKey(id));
          if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
          {
              return null;
          }

          return response.Resource;
      }

      public async Task<IEnumerable<RecordingEntry>> GetItemsAsync(string queryString)
      {
          var query = this._container.GetItemQueryIterator<RecordingEntry>(new QueryDefinition(queryString));
          List<RecordingEntry> results = new List<RecordingEntry>();
          while (query.HasMoreResults)
          {
              var response = await query.ReadNextAsync();
              
              results.AddRange(response.ToList());
          }

          return results;
      }
    }
}
