using System;
using System.Collections.Generic;

namespace ElasticSearch.Entities.ECommerce
{
    public class CustomerUser
    {
        public List<string> Category { get; set; }
        public string Currency { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerGender { get; set; }
        public int CustomerId { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhone { get; set; }
        public string DayOfWeek { get; set; }
        public int DayOfWeekI { get; set; }
        public string Email { get; set; }
        public List<string> Manufacturer { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderId { get; set; }
        public List<Product> Products { get; set; }
        public List<string> Sku { get; set; }
        public double TaxfulTotalPrice { get; set; }
        public double TaxlessTotalPrice { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalUniqueProducts { get; set; }
        public string Type { get; set; }
        public string User { get; set; }
        public Geoip Geoip { get; set; }
    }
}
