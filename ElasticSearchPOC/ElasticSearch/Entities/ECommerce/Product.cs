using Nest;
using System;

namespace ElasticSearch.Entities.ECommerce
{
    public class Product
    {
        public double BasePrice { get; set; }
        public int DiscountPercentage { get; set; }
        public int Quantity { get; set; }
        public string Manufacturer { get; set; }
        public int TaxAmount { get; set; }
        public int ProductId { get; set; }
        public string Category { get; set; }
        public string Sku { get; set; }
        public double TaxlessPrice { get; set; }
        public int UnitDiscountAmount { get; set; }
        public double MinPrice { get; set; }
        [PropertyName("_id")]
        public string Id { get; set; }
        public int DiscountAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public double TaxfulPrice { get; set; }
        public double BaseUnitPrice { get; set; }
    }

    public class Location
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
    }

    public class Geoip
    {
        public string CountryIsoCode { get; set; }
        public Location Location { get; set; }
        public string RegionName { get; set; }
        public string ContinentName { get; set; }
        public string CityName { get; set; }
    }
}
