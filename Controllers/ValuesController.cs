using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using incrementally.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using models.recording;

namespace incrementally.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        public ValuesController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<IEnumerable<Recording>> Get()
        {
            return await _cosmosDbService.GetItemsAsync("SELECT * FROM c");
        }

        [HttpGet]
        [Route("test")]
        public string Test()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
        
        [HttpPost]
        [Route("create")]
        public async Task<Recording> CreateAsync(RecordingData recording)
        {
            var item = new Recording();
            item.Data = recording.Data;
            item.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            item.GivenName = User.FindFirstValue(ClaimTypes.GivenName);
            item.Surname = User.FindFirstValue(ClaimTypes.Surname);
            item.Id = Guid.NewGuid().ToString();
            await _cosmosDbService.AddItemAsync(item);
            return item;
        }
    }

    public class RecordingData {
        public string Data;
    }
}
