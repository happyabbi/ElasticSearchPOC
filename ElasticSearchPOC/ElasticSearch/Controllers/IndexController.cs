﻿using ElasticSearch.Entities.Employee;
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

        [HttpGet]
        [Route("POCO")]
        public async Task<IActionResult> IndexAutoMapPOCO()
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

        [HttpPost]
        [Route("POCO")]
        public async Task<IActionResult> IndexDocumentForPOCO([FromBody] Company companyData)
        {
            string newIndexName = $"{IndexName}_poco";
            var addNewIndex = await _elasticClient.IndexAsync(companyData, i => i.Index(newIndexName));
            if (addNewIndex.IsValid)
                return Ok(addNewIndex.Id);
            else
                return BadRequest(addNewIndex.ServerError.Error);
        }

        [HttpGet]
        [Route("attribute")]
        public async Task<IActionResult> IndexFromAttribute()
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

        [HttpPost]
        [Route("attribute")]
        public async Task<IActionResult> IndexDocumentForAttribute([FromBody] EmployeeWithAttribute companyData)
        {
            string newIndexName = $"{IndexName}_attribute";
            var addNewIndex = await _elasticClient.IndexAsync(companyData, i => i.Index(newIndexName));
            if (addNewIndex.IsValid)
                return Ok(addNewIndex.Id);
            else
                return BadRequest(addNewIndex.ServerError.Error);
        }

        [HttpGet]
        [Route("fluentattribute")]
        public async Task<IActionResult> IndexWithMappings()
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

        [HttpPost]
        [Route("fluentattribute")]
        public async Task<IActionResult> IndexDocumentForFluentAttribute([FromBody] Company companyData)
        {
            /// this method creates document but updates if id is already existing
            var addNewIndex = await _elasticClient.IndexAsync(companyData, i => i.Index(IndexName));
            if (addNewIndex.IsValid)
                return Ok(addNewIndex.Id);
            else
                return BadRequest(addNewIndex.ServerError.Error);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> IndexDocumentWithId([FromBody] Company companyData)
        {
            /// this will create document and if id is already there then throws a error
            /// id is mandatory in this scenario. it can be with the document or can be passed in request.
            var addNewIndex = await _elasticClient.CreateAsync(companyData, i => i.Index(IndexName).Id(Guid.NewGuid()));
            if (addNewIndex.IsValid)
                return Ok(addNewIndex.Id);
            else
                return BadRequest(addNewIndex.ServerError.Error);
        }

        [HttpPut]
        [Route("fluentattribute")]
        public async Task<IActionResult> UpdateWithReplaceDoc([FromBody] Company companyData)
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

        [HttpPatch]
        [Route("fluentattribute")]
        public async Task<IActionResult> UpdateWithPartialDoc([FromBody] Company companyData)
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

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteByID([FromBody] Company companyData)
        {
            try
            {
                var indexResponse = await _elasticClient.IndexAsync(companyData, i => i.Index(IndexName));
                var queryResponse = await _elasticClient.DeleteAsync(new Nest.DeleteRequest(IndexName, indexResponse.Id));
                if (queryResponse.IsValid)
                {
                    return Ok(queryResponse.Result.ToString());
                }
                return BadRequest(queryResponse.OriginalException.Message);

            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}