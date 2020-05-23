using Database.Models;
using incrementally.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace incrementally_backend.Application
{
    public class RecordingHandler
    {
        private readonly CosmosDbService _cosmosDbService;

        public RecordingHandler(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<IEnumerable<RecordingMetadata>> Delete(string userId, string entryId)
        {
            var entry = await _cosmosDbService.GetRecordingMetadata(entryId).ConfigureAwait(false);
            if (entry.Count() == 0)
            {
                throw new Exception("Item not found");
            }
            if (userId == entry.First().CreatedBy)
            {
                await _cosmosDbService.DeleteRecording(entryId).ConfigureAwait(false);
                return entry;
            } else
            {
                throw new Exception("User not authorized to delete this item");
            }
        }
    }
}
