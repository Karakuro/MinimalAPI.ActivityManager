using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalAPI.ActivityManager.Library
{
    public class Person
    {
        private const float MIN_SALARY = 1500;
        public readonly Guid Id;
        public Guid PersonId { get => Id; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public float Salary { get => Math.Max((int)Level * 100 + 1300, MIN_SALARY); }
        public WorkLevel Level { get; set; }

        public Person()
        {
            Id = Guid.NewGuid();
        }
        public override string ToString()
        {
            return $"{Name} {Surname}";
        }

        public string ToDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Id: {Id}");
            sb.AppendLine($"Nome e Cognome: {this}");
            sb.AppendLine($"Data di Nascita: {Birthday.ToShortDateString()}");
            sb.AppendLine($"Indirizzo: {Address}, {City}, {PostalCode}");
            sb.AppendLine($"Telefono: {PhoneNumber}");
            sb.AppendLine($"Salario: {Salary}");
            sb.AppendLine($"Livello: {Level}");
            return sb.ToString();
        }
    }
}
