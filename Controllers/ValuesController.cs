using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Models;
using incrementally.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Compilers;

namespace incrementally.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private readonly IDatabaseConnector _database;
        public ValuesController(IDatabaseConnector database)
        {
            _database = database;
        }

        [HttpGet]
        [AllowAnonymous]
        public string Get()
        {
            return "Server is running";
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("recording/{id?}")]
        public async Task<IEnumerable<RecordingEntry>> Recording(string id)
        {
            return await _database.GetRecordings(id);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("metadata/{id}")]
        public async Task<IEnumerable<RecordingMetadata>> SingleMetadata(string id)
        {
            return await _database.GetRecordingMetadata(id);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("metadata")]
        public async Task<IEnumerable<RecordingMetadata>> Metadata()
        {
            return await _database.GetTopRecordingMetadata();
        }

        [HttpGet]
        [Route("allRecordingsMetadata")]
        public async Task<IEnumerable<RecordingEntry>> getAll()
        {
            return await _database.GetItemsAsync($@" 
        SELECT 
          c.title,
          c.description,
          c.createdBy,
          c.givenName,
          c.surname,
          c.id
        FROM c
        WHERE c.CreatedBy = ""{User.FindFirst(ClaimTypes.NameIdentifier).Value}""
      ");
        }


        [HttpPost]
        [Route("create")]
        public async Task<RecordingMetadata> CreateAsync(UserRecordingInput data)
        {
            var recordingEntry = new RecordingEntry();
            var recordingMetadata = new RecordingMetadata();
            var id = Guid.NewGuid().ToString();
            recordingEntry.Recording = data.Recording;
            recordingEntry.Id = id;
            recordingMetadata.Title = data.Title;
            recordingMetadata.Description = data.Description;
            recordingMetadata.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            recordingMetadata.GivenName = User.FindFirstValue(ClaimTypes.GivenName);
            recordingMetadata.Surname = User.FindFirstValue(ClaimTypes.Surname);
            recordingMetadata.Id = id;
            recordingMetadata.CreatedAt = DateTimeOffset.Now;
            await _database.AddRecordingAsync(recordingEntry, recordingMetadata);
            return recordingMetadata;
        }
    }

    public class UserRecordingInput
    {
        public string Recording { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
