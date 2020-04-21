namespace ElasticSearch.ViewModel
{
    public class PostRequestBody
    {
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public string SortOrder { get; set; }
        public string SortField { get; set; }
    }
}
