using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using incrementally.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using models.recording;
using SqlKata;
using SqlKata.Compilers;

namespace incrementally.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly PostgresCompiler _compiler;
        public ValuesController(ICosmosDbService cosmosDbService, PostgresCompiler compiler)
        {
            _cosmosDbService = cosmosDbService;
            _compiler = compiler;
        }

        [HttpGet]
        [AllowAnonymous]
        public string Get()
        {
            return "Server is running";
        }

        [HttpGet]
        [Route("recording/{id?}")]
        public async Task<IEnumerable<RecordingEntry>> Recording(string id)
        {
            _cosmosDbService.
            var query = new SqlQuerySpec
            {
                QueryText = "SELECT * FROM books b WHERE (b.Author.Name = @name)", 
                Parameters = new SqlParameterCollection() 
                { 
                    new SqlParameter("@name", "Herman Melville")
                }
            }
            var query = new Query("c").When(
                id != null,
                q => q.Where("c.id", id)
            );
            var result =  _compiler.Compile(query);
            return await _cosmosDbService.GetItemsAsync(result.Sql.Replace("\"c\"", "c").Replace("\".\"", "."));
        }

        [HttpGet]
        [Route("metadata/{userId?}/{id?}")]
        public async Task<IEnumerable<RecordingEntry>> Metadata(string userId, string id)
        {
            var query = new Query("c")
                .Select("c.title", "c.description", "c.createdBy", "c.givenName", "c.surname", "c.id")
                .When(
                    userId != null,
                    q => q.Where("c.createdBy", userId)
                )
                .When(
                    id != null,
                    q => q.Where("c.id", id)
                );
            var result = _compiler.Compile(query);
            return await _cosmosDbService.GetItemsAsync(result.Sql.Replace("\"c\"", "c"));
            // WHERE c.CreatedBy = ""{User.FindFirst(ClaimTypes.NameIdentifier).Value}""
        }

        [HttpGet]
        [Route("allRecordingsMetadata")]
        public async Task<IEnumerable<RecordingEntry>> getAll()
        {
            return await _cosmosDbService.GetItemsAsync($@" 
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
        public async Task<RecordingEntry> CreateAsync(RecordingData recording)
        {
            var item = new RecordingEntry();
            item.Recording = recording.Recording;
            item.Title = recording.Title;
            item.Description = recording.Description;
            item.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            item.GivenName = User.FindFirstValue(ClaimTypes.GivenName);
            item.Surname = User.FindFirstValue(ClaimTypes.Surname);
            item.Id = Guid.NewGuid().ToString();
            await _cosmosDbService.AddItemAsync(item);
            return item;
        }
    }

    public class RecordingData {
        public string Recording;
        public string Title;
        public string Description;
    }
}
