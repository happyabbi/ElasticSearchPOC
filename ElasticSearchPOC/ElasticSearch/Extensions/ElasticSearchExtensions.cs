using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
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
            var config = new ConnectionSettings(connectionPool);

            var client = new ElasticClient(config);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
