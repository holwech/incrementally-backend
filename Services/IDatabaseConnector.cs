using Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace incrementally.Services
{
    public interface IDatabaseConnector
    {
        Task InitializeAsync(string databaseName, List<string> containerNames, string account, string key);
        Task CreateContainer(string containerName);
        Task AddRecordingAsync(RecordingMetadata recordingMetadata);
        Task<IEnumerable<RecordingMetadata>> GetTopRecording();
        Task<RecordingMetadata> GetRecording(string id);
    }
}