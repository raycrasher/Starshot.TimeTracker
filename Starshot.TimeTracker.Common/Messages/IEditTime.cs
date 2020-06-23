using System;
using System.Collections.Generic;
using System.Text;

namespace Starshot.TimeTracker.Messages
{
    public interface IEditTime
    {
        int TimeRecordId { get; }
        DateTimeOffset NewTime { get; }
    }
}
