using System;

namespace Starshot.TimeTracker.Models
{
    public class TimeRecord
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTimeOffset TimeIn { get; set; }
        public DateTimeOffset? TimeOut { get; set; }

        public string EmployeeLastName { get; set; }
        public string EmployeeFirstName { get; set; }

        public bool IsTimedIn { get; set; }
    }
}
