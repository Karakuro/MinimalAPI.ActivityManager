using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalAPI.ActivityManager.Library
{
    public class Activity
    {
        public readonly Guid Id;
        public readonly DateTime CreationDate;
        public Person Manager { get; set; }
        public List<Person> Employees { get; set; }
        public string Title { get; set; }
        public float Budget { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ActivityType Type { get; set; }
        public bool IsCritical { get; set; }

        public Activity()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
            Employees = new List<Person>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Titolo: {Title}");
            sb.AppendLine($"Manager: {Manager}");
            if (Employees.Any())
            {
                sb.AppendLine("Dipendenti:\n");
                sb.AppendLine(string.Join("\n\t- ", Employees));
                //foreach (Person person in Employees)
                //{
                //    sb.AppendLine($"\t- {person}");
                //}
            }
            return sb.ToString();
        }
    }
}
