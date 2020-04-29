using Nest;
using System;

namespace ElasticSearch.Entities.ECommerce
{
    public class Product
    {
        [PropertyName("base_price")]
        public double BasePrice { get; set; }
        [PropertyName("discount_percentage")]
        public int DiscountPercentage { get; set; }
        [PropertyName("quantity")]
        public int Quantity { get; set; }
        [PropertyName("manufacturer")]
        public string Manufacturer { get; set; }
        [PropertyName("tax_amount")]
        public int TaxAmount { get; set; }
        [PropertyName("product_id")]
        public int ProductId { get; set; }
        [PropertyName("category")]
        public string Category { get; set; }
        [PropertyName("sku")]
        public string Sku { get; set; }
        [PropertyName("taxless_price")]
        public double TaxlessPrice { get; set; }
        [PropertyName("unit_discount_amount")]
        public int UnitDiscountAmount { get; set; }
        [PropertyName("min_price")]
        public double MinPrice { get; set; }
        [PropertyName("_id")]
        public string Id { get; set; }
        [PropertyName("discount_amount")]
        public int DiscountAmount { get; set; }
        [PropertyName("created_on")]
        public DateTime CreatedOn { get; set; }
        [PropertyName("product_name")]
        public string ProductName { get; set; }
        [PropertyName("price")]
        public double Price { get; set; }
        [PropertyName("taxful_price")]
        public double TaxfulPrice { get; set; }
        [PropertyName("base_unit_price")]
        public double BaseUnitPrice { get; set; }
    }

    public class Location
    {
        [PropertyName("lon")]
        public double Lon { get; set; }
        [PropertyName("lat")]
        public double Lat { get; set; }
    }

    public class Geoip
    {
        [PropertyName("country_iso_code")]
        public string CountryIsoCode { get; set; }
        [PropertyName("location")]
        public Location Location { get; set; }
        [PropertyName("region_name")]
        public string RegionName { get; set; }
        [PropertyName("continent_name")]
        public string ContinentName { get; set; }
        [PropertyName("city_name")]
        public string CityName { get; set; }
    }
}
