using System;
using System.Collections.Generic;

namespace ElasticSearch.Entities
{
    public class CustomerUser
    {
        public List<string> category { get; set; }
        public string currency { get; set; }
        public string customer_first_name { get; set; }
        public string customer_full_name { get; set; }
        public string customer_gender { get; set; }
        public int customer_id { get; set; }
        public string customer_last_name { get; set; }
        public string customer_phone { get; set; }
        public string day_of_week { get; set; }
        public int day_of_week_i { get; set; }
        public string email { get; set; }
        public List<string> manufacturer { get; set; }
        public DateTime order_date { get; set; }
        public int order_id { get; set; }
        public List<Product> products { get; set; }
        public List<string> sku { get; set; }
        public double taxful_total_price { get; set; }
        public double taxless_total_price { get; set; }
        public int total_quantity { get; set; }
        public int total_unique_products { get; set; }
        public string type { get; set; }
        public string user { get; set; }
        public Geoip geoip { get; set; }
    }
}
