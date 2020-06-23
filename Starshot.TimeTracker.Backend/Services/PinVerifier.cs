using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Starshot.TimeTracker.Backend.Services
{
    public interface IPinVerifier
    {
        Task<bool> VerifyPin(int employeeId, int pin);
    }

    public class PinVerifier : IPinVerifier
    {
        public Task<bool> VerifyPin(int employeeId, int pin)
        {
            return Task.FromResult(pin == 1234);
        }
    }
}
