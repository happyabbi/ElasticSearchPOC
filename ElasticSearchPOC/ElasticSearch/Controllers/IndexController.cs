using ElasticSearch.Entities.Employee;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
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
            // delete the index if it exists. Useful for demo purposes so that
            // we can re-run this example.
            if (_elasticClient.Indices.Exists(IndexName).Exists)
                _elasticClient.Indices.Delete(IndexName);

            // create the index, adding the mapping for the Page type to the index
            // at the same time. Automap() will infer the mapping from the POCO
            var createIndexResponse = await _elasticClient.Indices.CreateAsync(IndexName, c => c
                .Map<Company>(m => m
                .AutoMap(typeof(Employee))));
            if (createIndexResponse.IsValid)
            {
                var mappingRequest = new GetMappingRequest()
                {
                    Pretty = true
                };
                var mapping = await _elasticClient.Indices.GetMappingAsync(mappingRequest);
                mapping.Indices.TryGetValue(IndexName, out IndexMappings indexMappings);
                return Ok(JsonConvert.SerializeObject(indexMappings.Mappings.Properties.Count));
            }
            else
                return BadRequest(createIndexResponse.OriginalException.Message);
        }

        [HttpGet]
        [Route("attribute")]
        public async Task<IActionResult> IndexFromAttribute()
        {
            // delete the index if it exists. Useful for demo purposes so that
            // we can re-run this example.
            if (_elasticClient.Indices.Exists(IndexName).Exists)
                _elasticClient.Indices.Delete(IndexName);

            var createIndexResponse = await _elasticClient.Indices.CreateAsync(IndexName, c => c
                .Map<EmployeeWithAttribute>(m => m));
            if (createIndexResponse.IsValid)
            {
                return Ok(createIndexResponse.Index);
            }
            else
                return BadRequest(createIndexResponse.OriginalException.Message);
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
                return Ok(createIndexResponse.Index);
            }
            else
                return BadRequest(createIndexResponse.OriginalException.Message);
        }
    }
}