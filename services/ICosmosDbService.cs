namespace incrementally.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using models.recording;

  public interface ICosmosDbService
    {
      Task<IEnumerable<RecordingEntry>> GetItemsAsync(string query);
      Task<RecordingEntry> GetItemAsync(string id);
      Task AddItemAsync(RecordingEntry item);
      // Task UpdateItemAsync(string id, Recording item);
      // Task DeleteItemAsync(string id);
    }
}