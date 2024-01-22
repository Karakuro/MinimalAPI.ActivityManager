using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalAPI.ActivityManager.Library
{
    public class Person
    {
        public readonly Guid Id;
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public float Salary { get; set; }
        public WorkLevel Level { get; set; }

        public Person()
        {
            Id = Guid.NewGuid();
        }
        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
