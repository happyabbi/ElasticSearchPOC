using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearch.Extensions
{
    public static class ElasticSearchExtensions
    {
        public static void AddElasticSearch(
            this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("ElasticSearchConfig:Url");
            List<string> url = section.Get<List<string>>();

            var nodes = url.Select(x => new Uri(x));
            var connectionPool = new SniffingConnectionPool(nodes);

            // This is to set the default seralization settings for all the documents.
            // SnakeCaseNaming changes FilePath to file_path
            var config = new ConnectionSettings(connectionPool, 
                sourceSerializer: (builtin, settings) => new JsonNetSerializer(builtin, settings, () => new JsonSerializerSettings 
                { NullValueHandling = NullValueHandling.Include }, resolver => resolver.NamingStrategy = new SnakeCaseNamingStrategy()));

            var client = new ElasticClient(config);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
