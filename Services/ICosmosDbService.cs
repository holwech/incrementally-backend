namespace incrementally.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Database.Models;

  public interface ICosmosDbService
    {
      Task<IEnumerable<RecordingEntry>> GetItemsAsync(string query);
      Task<List<RecordingEntry>> GetRecordings(string id);
      Task<RecordingEntry> GetItemAsync(string id);
      Task AddItemAsync(RecordingEntry item);
      // Task UpdateItemAsync(string id, Recording item);
      // Task DeleteItemAsync(string id);
    }
}