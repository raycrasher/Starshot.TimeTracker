﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Starshot.TimeTracker.Messages
{
    public interface IEmployeeTimedOut
    {
        int EmployeeId { get; }
        DateTimeOffset Time { get; }
    }
}
