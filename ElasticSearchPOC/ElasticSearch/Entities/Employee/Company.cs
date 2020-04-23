using Nest;
using System;
using System.Collections.Generic;

namespace ElasticSearch.Entities.Employee
{
    public abstract class Document
    {
        public JoinField Join { get; set; }
    }

    public class Company : Document
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyLocation { get; set; }
        public List<Employee> Employees { get; set; }
    }

    public class Employee : Document
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Salary { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsManager { get; set; }
        public List<Employee> Employees { get; set; }
        public TimeSpan Hours { get; set; }
    }
}
