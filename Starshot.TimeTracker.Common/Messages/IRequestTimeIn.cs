﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Starshot.TimeTracker.Messages
{
    public interface IRequestTimeIn
    {
        int EmployeeId { get; }
        int Pin { get; }
    }
}
