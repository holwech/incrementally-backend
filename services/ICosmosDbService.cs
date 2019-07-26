namespace incrementally.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using models.recording;

  public interface ICosmosDbService
    {
      Task<IEnumerable<Recording>> GetItemsAsync(string query);
      Task<Recording> GetItemAsync(string id);
      Task AddItemAsync(Recording item);
      // Task UpdateItemAsync(string id, Recording item);
      // Task DeleteItemAsync(string id);
    }
}