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

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/writing-queries.html#unstructured-search
        /// Used generally to search within full text fields in order to find most relevant documents.
        /// Usually part of full text queries and the query input is analyzed.
        /// Analyzers can be applied to text datatype fields using mapping
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("unstructured")]
        public async Task<IActionResult> UnStructuredFullTextQuery()
        {
            try
            {
                // since we are using snake naming conventions fields mentioned in queries do not follow naming convention defined.
                // hence a relevant default infer convention is specified with connection or property name is added in class fields
                // refer https://github.com/elastic/elasticsearch-net/issues/4121

                var queryResponse = await _elasticClient.SearchAsync<CustomerUser>(s => s
                                                        .Index(IndexName)
                                                        .Query(q => q
                                                                    .Match(m => m
                                                                                .Field(f => f.CustomerFullName)
                                                                                .Query("Eddie"))));

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

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/writing-queries.html#unstructured-search
        /// Used generally to search within full text fields in order to find most relevant documents.
        /// Usually part of full text queries and the query input is analyzed.
        /// Analyzers can be applied to text datatype fields using mapping
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("unstructurednested")]
        public async Task<IActionResult> UnStructuredFullTextQueryNested()
        {
            try
            {
                var queryResponse = await _elasticClient.SearchAsync<CustomerUser>(s => s
                                                        .Index(IndexName)
                                                        .Query(q => q
                                                                    .Match(m => m
                                                                                .Field(f => f.Geoip.ContinentName)
                                                                                .Query("Asia"))));

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

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/writing-queries.html#combining-queries
        /// This combines the structured and unstructured queries to perform search operations
        /// Here in this example the operation is to find customers who have placed orders between date range and belong to North America continent along with Los Angeles city
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("compound")]
        public async Task<IActionResult> CompoundQuery()
        {
            try
            {
                var queryResponse = await _elasticClient
                    .SearchAsync<CustomerUser>(s => s.Index(IndexName)
                                                     .Query(q => q
                                                         .Bool(b => b
                                                            .Must(mu => mu
                                                                .Match(m => m.Field(f => f.Geoip.ContinentName)
                                                                             .Query("North America")
                                                                ), mu => mu
                                                                .Match(m => m.Field(f => f.Geoip.CityName)
                                                                             .Query("Los Angeles")
                                                                )
                                                            )
                                                            .Filter(fi => fi
                                                                .DateRange(r => r
                                                                    .Field(f => f.OrderDate)
                                                                    .GreaterThanOrEquals(new DateTime(2020, 3, 20))
                                                                    .LessThan(DateTime.Now)
                                                                )))));

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