using Nest;
using System;
using System.Collections.Generic;

namespace ElasticSearch.Entities.ECommerce
{
    public class CustomerUser
    {
        [PropertyName("category")]
        public List<string> Category { get; set; }
        [PropertyName("currency")]
        public string Currency { get; set; }
        [PropertyName("customer_first_name")]
        public string CustomerFirstName { get; set; }
        [PropertyName("customer_full_name")]
        public string CustomerFullName { get; set; }
        [PropertyName("customer_gender")]
        public string CustomerGender { get; set; }
        [PropertyName("customer_id")]
        public int CustomerId { get; set; }
        [PropertyName("customer_last_name")]
        public string CustomerLastName { get; set; }
        [PropertyName("customer_phone")]
        public string CustomerPhone { get; set; }
        [PropertyName("day_of_week")]
        public string DayOfWeek { get; set; }
        [PropertyName("day_of_week_i")]
        public int DayOfWeekI { get; set; }
        [PropertyName("email")]
        public string Email { get; set; }
        [PropertyName("manufacturer")]
        public List<string> Manufacturer { get; set; }
        [PropertyName("order_date")]
        public DateTime OrderDate { get; set; }
        [PropertyName("order_id")]
        public int OrderId { get; set; }
        [PropertyName("products")]
        public List<Product> Products { get; set; }
        [PropertyName("sku")]
        public List<string> Sku { get; set; }
        [PropertyName("taxful_total_price")]
        public double TaxfulTotalPrice { get; set; }
        [PropertyName("taxless_total_price")]
        public double TaxlessTotalPrice { get; set; }
        [PropertyName("total_quantity")]
        public int TotalQuantity { get; set; }
        [PropertyName("total_unique_products")]
        public int TotalUniqueProducts { get; set; }
        [PropertyName("type")]
        public string Type { get; set; }
        [PropertyName("user")]
        public string User { get; set; }
        [PropertyName("geoip")]
        public Geoip Geoip { get; set; }
    }
}
