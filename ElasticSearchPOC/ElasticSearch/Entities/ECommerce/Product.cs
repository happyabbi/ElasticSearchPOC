using System;

namespace ElasticSearch.Entities.ECommerce
{
    public class Product
    {
        public double base_price { get; set; }
        public int discount_percentage { get; set; }
        public int quantity { get; set; }
        public string manufacturer { get; set; }
        public int tax_amount { get; set; }
        public int product_id { get; set; }
        public string category { get; set; }
        public string sku { get; set; }
        public double taxless_price { get; set; }
        public int unit_discount_amount { get; set; }
        public double min_price { get; set; }
        public string _id { get; set; }
        public int discount_amount { get; set; }
        public DateTime created_on { get; set; }
        public string product_name { get; set; }
        public double price { get; set; }
        public double taxful_price { get; set; }
        public double base_unit_price { get; set; }
    }

    public class Location
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Geoip
    {
        public string country_iso_code { get; set; }
        public Location location { get; set; }
        public string region_name { get; set; }
        public string continent_name { get; set; }
        public string city_name { get; set; }
    }
}
