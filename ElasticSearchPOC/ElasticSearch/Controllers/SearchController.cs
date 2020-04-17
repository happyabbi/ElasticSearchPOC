using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Threading.Tasks;

namespace ElasticSearch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly string IndexName = "kibana_sample_data_ecommerce";

        public SearchController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// This API is used to fetch all records against the index.
        /// This type of fetching should be avoided with increase in number of records.
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public async Task<IActionResult> GetAllDocuments()
        {
            try
            {
                var queryResponse = await _elasticClient
                        .CountAsync<object>(s => s.Index(IndexName));

                var recordsCount = queryResponse.Count;

                var allRecords = await _elasticClient
                        .SearchAsync<object>(s => s.Index(IndexName).Size(Convert.ToInt32(recordsCount)).MatchAll());

                var responseObject = new Entities.SearchResponse
                {
                    RecordCount = recordsCount,
                    Records = allRecords.Documents
                };

                return Ok(responseObject);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
