using ElasticSearch.Entities;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElasticSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly string IndexName = "kibana_sample_data_ecommerce";

        public AggregationController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// Sample for Bucket Adjacency Matrix Aggregation
        /// Refer : https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations-bucket-adjacency-matrix-aggregation.html#search-aggregations-bucket-adjacency-matrix-aggregation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("bucket/matrix")]
        public async Task<IActionResult> PerformBucketAdjacentMatrix()
        {
            try
            {
                var agg = new AdjacencyMatrixAggregation("manufactures")
                {
                    Filters = new NamedFiltersContainer()
                    {
                         { "grpA", new TermsQuery { Field = "manufacturer.keyword", Terms = new string [2] { "Elitelligence", "Oceanavigations" } } },
                         { "grpB", new TermsQuery { Field = "manufacturer.keyword", Terms = new string [2] { "Elitelligence", "Pyramidustries" } } },
                         { "grpC", new TermsQuery { Field = "manufacturer.keyword", Terms = new string [2] { "Champion Arts", "Pyramidustries" } } },
                    }
                };
                var searchRequest = new SearchRequest(IndexName)
                {
                    Aggregations = agg,
                    Size = 0
                };
                var searchResponse = await _elasticClient.SearchAsync<object>(searchRequest);
                if (searchResponse.IsValid)
                {
                    var bucketAggregate = ((BucketAggregate)searchResponse.Aggregations["manufactures"]).Items;

                    var responseList = new List<AggregateResponse>();
                    foreach (var bucket in bucketAggregate)
                    {
                        var item = (KeyedBucket<object>)bucket;
                        responseList.Add(new AggregateResponse() { DocCount = item.DocCount, Group = item.Key.ToString() });
                    }

                    return Ok(responseList);
                }
                else
                    return BadRequest(searchResponse.ServerError.Error);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Sample for Bucket Auto-interval Date Histogram Aggregation
        /// Refer : https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations-bucket-autodatehistogram-aggregation.html
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("bucket/autodatehistogram/{bucketSize}")]
        public async Task<IActionResult> PerformBucketAutoDateHistogram(int bucketSize)
        {
            try
            {
                var agg = new AutoDateHistogramAggregation("autoDateHistogram")
                {
                   Field = "order_date",
                   Buckets = bucketSize,
                   Format = "yyyy-MM-dd",
                   Missing = DateTime.Now.AddYears(-2),
                   MinimumInterval = MinimumInterval.Hour
                };
                var searchRequest = new SearchRequest(IndexName)
                {
                    Aggregations = agg,
                    Size = 0
                };
                var searchResponse = await _elasticClient.SearchAsync<object>(searchRequest);
                if (searchResponse.IsValid)
                {
                    var bucketAggregate = (BucketAggregate)searchResponse.Aggregations["autoDateHistogram"];

                    var responseList = new List<AggregateResponse>();
                    foreach (var bucket in bucketAggregate.Items)
                    {
                        var item = (DateHistogramBucket)bucket;
                        responseList.Add(new AggregateResponse() { DocCount = item.DocCount, Group = item.KeyAsString, Interval = bucketAggregate.Interval.Factor + bucketAggregate.Interval.Interval.Value.GetStringValue() });
                    }

                    return Ok(responseList);
                }
                else
                    return BadRequest(searchResponse.ServerError.Error);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}