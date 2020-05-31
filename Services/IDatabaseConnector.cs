using Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace incrementally.Services
{
    public interface IDatabaseConnector
    {
        Task InitializeAsync(string databaseName, List<string> containerNames, string account, string key);
        Task AddRecordingAsync(RecordingEntry recordingEntry, RecordingMetadata recordingMetadata);
        Task<List<RecordingEntry>> GetRecordings(string id);
        Task<RecordingEntry> GetItemAsync(string id);
        Task<IEnumerable<RecordingEntry>> GetItemsAsync(string queryString);
        Task<IEnumerable<RecordingMetadata>> GetTopRecordingMetadata();
        Task<IEnumerable<RecordingMetadata>> GetRecordingMetadata(string id);
    }
}