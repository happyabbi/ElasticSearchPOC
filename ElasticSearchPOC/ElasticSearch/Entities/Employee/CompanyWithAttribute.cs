using Nest;
using System;
using System.Collections.Generic;

namespace ElasticSearch.Entities.Employee
{
    [ElasticsearchType(RelationName = "employee")]
    public class EmployeeWithAttribute
    {
        [Text(Name = "first_name", Norms = false)]
        public string FirstName { get; set; }

        [Text(Name = "last_name")]
        public string LastName { get; set; }

        [Number(DocValues = false, IgnoreMalformed = true, Coerce = true)]
        public int Salary { get; set; }

        [Date(Format = "MM-dd-yyyy")]
        public string Birthday { get; set; }

        [Boolean(NullValue = false, Store = true)]
        public bool IsManager { get; set; }

        [Nested]
        [PropertyName("empl")]
        public List<EmployeeWithAttribute> Employees { get; set; }

        [Text(Name = "office_hours")]
        public TimeSpan? OfficeHours { get; set; }

        [Object]
        public List<Skill> Skills { get; set; }
    }

    public class Skill
    {
        [Text]
        public string Name { get; set; }

        [Number(NumberType.Byte, Name = "level")]
        public int Proficiency { get; set; }
    }
}
