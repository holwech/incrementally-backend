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
      
      public async Task AddItemAsync(Recording item)
      {
          await this._container.CreateItemAsync<Recording>(item, new PartitionKey(item.Id));
      }

      public async Task<Recording> GetItemAsync(string id)
      {
          ItemResponse<Recording> response = await this._container.ReadItemAsync<Recording>(id, new PartitionKey(id));
          if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
          {
              return null;
          }

          return response.Resource;
      }

      public async Task<IEnumerable<Recording>> GetItemsAsync(string queryString)
      {
          var query = this._container.GetItemQueryIterator<Recording>(new QueryDefinition(queryString));
          List<Recording> results = new List<Recording>();
          while (query.HasMoreResults)
          {
              var response = await query.ReadNextAsync();
              
              results.AddRange(response.ToList());
          }

          return results;
      }
    }
}
