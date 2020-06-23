using Starshot.TimeTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Starshot.TimeTracker.Messages
{
    public interface IAddEmployee
    {
        Employee Employee { get; }
    }
}
