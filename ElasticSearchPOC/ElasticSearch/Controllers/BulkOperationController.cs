using ElasticSearch.Entities.Employee;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulkOperationController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly string IndexName = "employee";

        public BulkOperationController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        /// <summary>
        /// This API used for inserting bulk record to index name 
        /// Reference : https://www.elastic.co/guide/en/elasticsearch/client/net-api/1.x/bulk.html
        /// </summary>
        /// <param name="companies"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("bulkInsert")]
        public async Task<IActionResult> BulkInsert([FromBody] List<Company> companies)
        {
            try
            {
                var queryResponse = await _elasticClient.BulkAsync(x => x
                .CreateMany(companies)
                .Index(IndexName));
                if (queryResponse.IsValid)
                    return Ok(queryResponse.Items.Count);
                return BadRequest(queryResponse.ServerError.Error);
            }
            catch (System.Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        /// <summary>
        /// This method is used for bulk updating the document based on Id
        /// Reference : https://www.elastic.co/guide/en/elasticsearch/client/net-api/1.x/bulk.html
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("bulkUpdate")]
        public async Task<IActionResult> BulkUpdate()
        {
            try
            {
                var indexResponse = await _elasticClient.SearchAsync<object>(x => x.Index(IndexName).MatchAll());
                var docs = indexResponse.Hits.Select(x => new { x.Id, Name = "zaffar" });

                var queryResponse = await _elasticClient.BulkAsync(x => x
                   .Index(IndexName)
                   .UpdateMany(docs, (bu, d) => bu.Doc(d)));

                if (queryResponse.IsValid)
                    return Ok(queryResponse.Items.Count);

                return BadRequest(queryResponse.ServerError.Error);

            }
            catch (System.Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This API is used for Deleting the bulk record
        /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/1.x/delete.html
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> BulkDelete()
        {
            try
            {
                var dummbyData = new List<Company>() {
                  new Company()
                  { Id = "1", Name = "abc", CompanyLocation = "banglore" },
                    new Company()
                  { Id = "2", Name = "xyz", CompanyLocation = "mysuru"}
                };
                var indexRespone = await _elasticClient.BulkAsync(bb => bb
                                                                          .CreateMany(dummbyData)
                                                                          .Index(IndexName));
                var list = indexRespone.Items.Select(x => x.Id);
                var queryResponse = await _elasticClient.BulkAsync(bb => bb
                                                                           .DeleteMany<Company>(list.Select(x => new Company { Id = x }))
                                                                           .Index(IndexName));
                if (queryResponse.IsValid)
                    return Ok(queryResponse.Items.Count);
                return BadRequest(queryResponse.ServerError.Error);
            }
            catch (System.Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This API gives the example of performing CURD opertion in single Bulk call.
        /// Reference : https://www.elastic.co/guide/en/elasticsearch/client/net-api/1.x/bulk.html
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("bulk")]
        public async Task<IActionResult> BulkAllOperation()
        {

            try
            {

                var bulkResponse = await _elasticClient.BulkAsync(b => b
                                                                        .Index<object>(i => i
                                                                            .Index(IndexName)
                                                                            .Id("1")
                                                                            .Document(new { Name = "abc" })
                                                                        )
                                                                        .Delete<Company>(d => d
                                                                            .Index(IndexName)
                                                                            .Id("2")
                                                                        )
                                                                        .Create<Company>(c => c
                                                                            .Index(IndexName)
                                                                            .Id("3")
                                                                            .Document(new Company { Name = "zaffar" })
                                                                        )
                                                                        .Update<Company>(u => u
                                                                            .Index(IndexName)
                                                                            .Id("1")
                                                                            .Doc(new Company { Name = "Infrrd.ai" })
                                                                        )
                                                                    );
                if (bulkResponse.IsValid)
                    return Ok(bulkResponse.Items.Count);
                return BadRequest(bulkResponse.ServerError);
            }
            catch (System.Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This API call is used for adding multiple document which act as bulk operation but not good recommend for more than 1000 doc 
        /// Reference  : https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/indexing-documents.html
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("mulitDoc")]
        public async Task<IActionResult> MultiDocument([FromBody] List<Company> company)
        {
            try
            {
                var queryResponse = await _elasticClient.IndexManyAsync(company, IndexName);
                if (queryResponse.IsValid)
                    return Ok(queryResponse.Items.Count);
                return BadRequest(queryResponse.ServerError.Error);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}