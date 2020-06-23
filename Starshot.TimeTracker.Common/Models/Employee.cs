using System;
using System.Collections.Generic;
using System.Text;

namespace Starshot.TimeTracker.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public bool IsActive { get; set; }
        public int? CurrentTimeInRecordId { get; set; }
    }
}
