using ElasticSearch.Entities.Employee;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace ElasticSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly string IndexName = "employee";

        public IndexController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// Create a Index with POCO objects.
        /// In this it infers mappings from the classes defined
        /// refer : https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/auto-map.html
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("POCO")]
        public async Task<IActionResult> IndexAutoMapPOCO()
        {
            try
            {
                string newIndexName = $"{IndexName}_poco";
                // delete the index if it exists. Useful for demo purposes so that
                // we can re-run this example.

                if (_elasticClient.Indices.Exists(newIndexName).Exists)
                    _elasticClient.Indices.Delete(newIndexName);

                // create the index, adding the mapping for the Page type to the index
                // at the same time. Automap() will infer the mapping from the POCO
                var createIndexResponse = await _elasticClient.Indices.CreateAsync(newIndexName, c => c
                    .Map<Document>(m => m
                    .AutoMap<Company>()
                    .AutoMap(typeof(Employee))));
                if (createIndexResponse.IsValid)
                {
                    var mappingRequest = new GetMappingRequest();
                    var mapping = await _elasticClient.Indices.GetMappingAsync(mappingRequest);
                    mapping.Indices.TryGetValue(newIndexName, out IndexMappings indexMappings);
                    return Ok(JsonConvert.SerializeObject(indexMappings.Mappings.Properties.Keys));
                }
                else
                    return BadRequest(createIndexResponse.ServerError.Error);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Insert a document against index created by POCO attribute mapping.
        /// </summary>
        /// <param name="companyData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("POCO")]
        public async Task<IActionResult> IndexDocumentForPOCO([FromBody] Company companyData)
        {
            try
            {
                string newIndexName = $"{IndexName}_poco";
                var addNewIndex = await _elasticClient.IndexAsync(companyData, i => i.Index(newIndexName));
                if (addNewIndex.IsValid)
                    return Ok(addNewIndex.Id);
                else
                    return BadRequest(addNewIndex.ServerError.Error);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a index with attributes defined against properties in classes.
        /// Prefared over POCO as it gives more control over mappings and properties for search
        /// Refer : https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/attribute-mapping.html
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("attribute")]
        public async Task<IActionResult> IndexFromAttribute()
        {
            try
            {
                string newIndexName = $"{IndexName}_attribute";
                // delete the index if it exists. Useful for demo purposes so that
                // we can re-run this example.
                if (_elasticClient.Indices.Exists(newIndexName).Exists)
                    _elasticClient.Indices.Delete(newIndexName);

                var createIndexResponse = await _elasticClient.Indices.CreateAsync(newIndexName, c => c
                    .Map<EmployeeWithAttribute>(m => m.AutoMap()));
                if (createIndexResponse.IsValid)
                {
                    var mappingRequest = new GetMappingRequest();
                    var mapping = await _elasticClient.Indices.GetMappingAsync(mappingRequest);
                    mapping.Indices.TryGetValue(newIndexName, out IndexMappings indexMappings);
                    return Ok(JsonConvert.SerializeObject(indexMappings.Mappings.Properties.Keys));
                }
                else
                    return BadRequest(createIndexResponse.ServerError.Error);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Insert a document against index created by attribute mapping.
        /// It is necessary over here to provide data as per format and other attributes defined.
        /// </summary>
        /// <param name="companyData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("attribute")]
        public async Task<IActionResult> IndexDocumentForAttribute([FromBody] EmployeeWithAttribute companyData)
        {
            try
            {
                // for this example the date format had to be same as what is defined which is MM-dd-yyyy
                string newIndexName = $"{IndexName}_attribute";
                var addNewIndex = await _elasticClient.IndexAsync(companyData, i => i.Index(newIndexName));
                if (addNewIndex.IsValid)
                    return Ok(addNewIndex.Id);
                else
                    return BadRequest(addNewIndex.ServerError.Error);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a index with attributes defined in request itself - fluent mapping.
        /// Attributes defined in request have more weightage then defined at class level
        /// refer : https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/fluent-mapping.html 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("fluentattribute")]
        public async Task<IActionResult> IndexWithMappings()
        {
            try
            {
                // delete the index if it exists. Useful for demo purposes so that
                // we can re-run this example.
                if (_elasticClient.Indices.Exists(IndexName).Exists)
                    _elasticClient.Indices.Delete(IndexName);

                var createIndexResponse = await _elasticClient.Indices.CreateAsync(IndexName, c => c
                    .Map<Company>(m => m
                        .Properties(ps => ps
                            .Text(s => s
                                .Name(n => n.Name))
                                .Object<Employee>(o => o
                                    .Name(n => n.Employees)
                                    .Properties(eps => eps
                                        .Text(s => s.Name(e => e.FirstName))
                                        .Text(s => s.Name(e => e.LastName))
                                        .Number(n => n.Name(e => e.Salary).Type(NumberType.Integer)))
                ))));
                if (createIndexResponse.IsValid)
                {
                    var mappingRequest = new GetMappingRequest();
                    var mapping = await _elasticClient.Indices.GetMappingAsync(mappingRequest);
                    mapping.Indices.TryGetValue(IndexName, out IndexMappings indexMappings);
                    return Ok(JsonConvert.SerializeObject(indexMappings.Mappings.Properties.Keys));
                }
                else
                    return BadRequest(createIndexResponse.ServerError.Error);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Insert a document against index created by fluent mapping.
        /// When used with id prefer CreateAsync to restrict updation of document instead of create.
        /// </summary>
        /// <param name="companyData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("fluentattribute")]
        public async Task<IActionResult> IndexDocumentForFluentAttribute([FromBody] Company companyData)
        {
            try
            {
                /// this method creates document but updates if id is already existing
                var addNewIndex = await _elasticClient.IndexAsync(companyData, i => i.Index(IndexName));
                if (addNewIndex.IsValid)
                    return Ok(addNewIndex.Id);
                else
                    return BadRequest(addNewIndex.ServerError.Error);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a document with the id provided.
        /// </summary>
        /// <param name="companyData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> IndexDocumentWithId([FromBody] Company companyData)
        {
            try
            {
                /// this will create document and if id is already there then throws a error
                /// id is mandatory in this scenario. it can be with the document or can be passed in request.
                var addNewIndex = await _elasticClient.CreateAsync(companyData, i => i.Index(IndexName).Id(Guid.NewGuid()));
                if (addNewIndex.IsValid)
                    return Ok(addNewIndex.Id);
                else
                    return BadRequest(addNewIndex.ServerError.Error);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates all the fields and add new fields if specified against the document.
        /// </summary>
        /// <param name="companyData"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("fluentattribute")]
        public async Task<IActionResult> UpdateWithReplaceDoc([FromBody] Company companyData)
        {
            try
            {
                var companyDocs = await _elasticClient.SearchAsync<object>(s =>
                                                        s.Index(IndexName).MatchAll());

                var enumerator = companyDocs.Hits.GetEnumerator();
                enumerator.MoveNext();

                var docId = enumerator.Current.Id;

                var addNewIndex = await _elasticClient.UpdateAsync<Company>(docId,
                                                          u => u.Index(IndexName)
                                                          .Doc(companyData));
                if (addNewIndex.IsValid)
                    return Ok(addNewIndex.Id);
                else
                    return BadRequest(addNewIndex.ServerError.Error);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates only the specified fields instead of entire document.
        /// If data loss has to reduced then this method should be used.
        /// </summary>
        /// <param name="companyData"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("fluentattribute")]
        public async Task<IActionResult> UpdateWithPartialDoc([FromBody] Company companyData)
        {
            try
            {
                var companyDocs = await _elasticClient.SearchAsync<object>(s =>
                                                        s.Index(IndexName).MatchAll());

                var enumerator = companyDocs.Hits.GetEnumerator();
                enumerator.MoveNext();

                var docId = enumerator.Current.Id;

                dynamic updateFields = new ExpandoObject();
                updateFields.is_active = false;
                updateFields.date_updated = DateTime.UtcNow;
                updateFields.company_location = companyData.CompanyLocation;

                var addNewIndex = await _elasticClient.UpdateAsync<object>(docId,
                                                          u => u.Index(IndexName)
                                                          .Doc(updateFields));
                if (addNewIndex.IsValid)
                    return Ok(addNewIndex.Id);
                else
                    return BadRequest(addNewIndex.ServerError.Error);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete the document as per the id.
        /// Here is order to reduce error in POC a create and then delete is used.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> DeleteByID()
        {
            try
            {
                var companyData = new Company()
                {
                    Name = "Test dummy"
                };
                var indexResponse = await _elasticClient.IndexAsync(companyData, i => i.Index(IndexName));
                var queryResponse = await _elasticClient.DeleteAsync(new Nest.DeleteRequest(IndexName, indexResponse.Id));
                if (queryResponse.IsValid)
                {
                    return Ok(queryResponse.Result);
                }
                return BadRequest(queryResponse.ServerError.Error);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}