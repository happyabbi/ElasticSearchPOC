using ElasticSearch.Entities.ECommerce;
using ElasticSearch.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ElasticSearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly string IndexName = "kibana_sample_data_ecommerce";

        public SearchController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        [Route("test")]
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("controller is accessible");
        }

        /// <summary>
        /// This API is used to fetch all records against the index.
        /// This type of fetching should be avoided with increase in number of records.
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  This API is used for sorting the primary level of data present in the indexes
        ///  Parameter value specify which field and sorting order to be done
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost, Route("sort")]
        public async Task<IActionResult> GetSorting([FromBody] PostRequestBody postRequestBody)
        {
            try
            {
                var sortOrder = postRequestBody.SortOrder != null && postRequestBody.SortOrder == "ASC" ? SortOrder.Ascending : SortOrder.Descending;
                var queryResponse = await _elasticClient.SearchAsync<object>(x => x.Index(IndexName)
                                                                                .Sort(ss => ss.Field(postRequestBody.SortField, sortOrder)));

                var responseObject = new Entities.SearchResponse
                {
                    RecordCount = queryResponse.HitsMetadata.Total.Value,
                    Records = queryResponse.Documents
                };
                return Ok(JsonConvert.SerializeObject(responseObject));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This API is used for fetching the record bases on the page size and page index
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="size"> to</param>
        /// <returns></returns>
        [HttpPost, Route("pagination")]
        public async Task<IActionResult> GetPagination([FromBody] PostRequestBody postRequestBody)
        {
            try
            {
                int from = 0;

                if(postRequestBody.PageIndex.HasValue)
                {
                    from = postRequestBody.PageIndex.Value - 1 * postRequestBody.PageSize.Value;
                }

                var queryResponse = await _elasticClient.SearchAsync<object>(x => x.Index(IndexName)
                                                                                   .From(from)
                                                                                   .Size(postRequestBody.PageSize));

                var responseObject = new Entities.SearchResponse
                {
                    RecordCount=queryResponse.HitsMetadata.Total.Value,
                    Records = queryResponse.Documents
                };
                return Ok(JsonConvert.SerializeObject(responseObject));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet,Route("multi")]
        public async Task<IActionResult> GetMultiSearch()
        {
            try
            {


                var allRecords = await _elasticClient
                        .SearchAsync<Product>(s => s
                        .Index(IndexName)
                        .MatchAll());
               

                return Ok(allRecords.Documents);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
