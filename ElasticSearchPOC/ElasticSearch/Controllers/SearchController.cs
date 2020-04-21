using ElasticSearch.Entities;
using Microsoft.AspNetCore.Mvc;
using Nest;
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
        //[HttpGet, Route("")]
        //public async Task<IActionResult> GetSearchByQueryString([FromQuery] string query)
        //{
        //    try
        //    {
        //        var queryResponse = await _elasticClient.SearchAsync<object>(x => x.Index(IndexName).QueryOnQueryString(query));
        //        var responseObject = new Entities.SearchResponse
        //        {
        //            Records = queryResponse.Documents
        //        };
        //        return Ok(responseObject);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}
        /// <summary>
        ///  Sorting based on field value but its not verified
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet, Route("sort")]
        public async Task<IActionResult> GetSortingValue([FromQuery]  string value)
        {
            try
            {
                var queryResponse = await _elasticClient.SearchAsync<object>(x => x.Index(IndexName).Sort(z => z.Field(p => p.Field(c => value))));
                var responseObject = new Entities.SearchResponse
                {
                    Records = queryResponse.Documents
                };
                return Ok(responseObject);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Pagination in base on doc of index 
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="size"> to</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetPagination(int from, int size)
        {
            try
            {
                var queryResponse = await _elasticClient.SearchAsync<object>(x => x.Index(IndexName).From(from).Size(size));
                var responseObject = new Entities.SearchResponse
                {
                    Records = queryResponse.Documents
                };
                return Ok(responseObject);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
