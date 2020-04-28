using ElasticSearch.Entities.ECommerce;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElasticSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly string IndexName = "kibana_sample_data_ecommerce";

        public QueryController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/writing-queries.html#structured-search
        /// Used generally for querying data which have a structure.
        /// Usually part of term level queries and the query input is not analyzed.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("structured")]
        public async Task<IActionResult> StructuredDateRangeQuery()
        {

            try
            {
                var queryContainer = new DateRangeQuery()
                {
                    Field = "order_date",
                    GreaterThanOrEqualTo = new DateTime(2020, 03, 16),
                    LessThan = DateMath.Now
                };
                IEnumerable<QueryContainer> queryContainers = new List<QueryContainer>() { queryContainer };
                var searchRequest = new SearchRequest<CustomerUser>(IndexName)
                {
                    Query = new BoolQuery()
                    {
                        Filter = queryContainers
                    }
                };

                var queryResponse = await _elasticClient.SearchAsync<CustomerUser>(searchRequest);

                if (queryResponse.IsValid)
                    return Ok(new Entities.SearchResponse()
                    {
                        RecordCount = queryResponse.HitsMetadata.Total.Value,
                        Records = queryResponse.Documents
                    });
                else
                    return BadRequest(queryResponse.ServerError.Error);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}