using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using incrementally.Services;
using Microsoft.AspNetCore.Mvc;
using models.recording;

namespace incrementally.Controllers
{
    [Route("api")]
    [ApiController]
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
            return "test";
        }
        
        [HttpGet]
        [Route("create")]
        public async Task<Recording> CreateAsync()
        {
            var item = new Recording();
            item.Id = Guid.NewGuid().ToString();
            await _cosmosDbService.AddItemAsync(item);
            return item;
        }
    }
}
