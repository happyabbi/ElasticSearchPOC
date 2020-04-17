using System.Collections.Generic;

namespace ElasticSearch.Entities
{
    public class SearchResponse
    {
        public long RecordCount { get; set; }
        public IReadOnlyCollection<object> Records { get; set; }
    }
}
