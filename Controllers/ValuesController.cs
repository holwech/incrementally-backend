using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Models;
using incrementally_backend.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace incrementally.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private readonly RecordingHandler _recordingHandler;

        public ValuesController(RecordingHandler recordingHandler)
        {
            _recordingHandler = recordingHandler;
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
        public async Task<RecordingEntry> Recording(string id)
        {
            return await _recordingHandler.GetRecording(id);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("metadata/{id}")]
        public async Task<RecordingMetadata> SingleMetadata(string id)
        {
            return await _recordingHandler.GetMetadata(id);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("metadata")]
        public async Task<IEnumerable<RecordingMetadata>> Metadata()
        {
            return await _recordingHandler.GetTopMetadata();
        }


        [HttpPost]
        [Route("create")]
        public async Task<RecordingMetadata> CreateAsync(UserRecordingInput data)
        {
            var recordingMetadata = new RecordingMetadata();
            var id = Guid.NewGuid().ToString();
            recordingMetadata.Title = data.Title;
            recordingMetadata.Description = data.Description;
            recordingMetadata.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            recordingMetadata.GivenName = User.FindFirstValue(ClaimTypes.GivenName);
            recordingMetadata.Surname = User.FindFirstValue(ClaimTypes.Surname);
            recordingMetadata.Id = id;
            recordingMetadata.CreatedAt = DateTimeOffset.Now;
            await _recordingHandler.Add(recordingMetadata, data.Recording);
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
