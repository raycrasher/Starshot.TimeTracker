using MassTransit;
using Starshot.TimeTracker.Backend.Services;
using Starshot.TimeTracker.Messages;
using Starshot.TimeTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Starshot.TimeTracker.Backend.Consumers
{
    public class RequestTimeOutConsumer : IConsumer<IEmployeeTimedOut>
    {
        ITimeLogRepository _repo;

        public RequestTimeOutConsumer(ITimeLogRepository repo)
        {
            _repo = repo;
        }

        public async Task Consume(ConsumeContext<IEmployeeTimedOut> context)
        {
            var msg = context.Message;

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Employee emp = await _repo.GetEmployeeById(msg.EmployeeId);
            if(emp == null)
            {
                await context.RespondAsync<ITimeInOutResponse>(new
                {
                    Success = false,
                    Message = "Employee does not exist"
                });
                return;
            }

            if(emp.CurrentTimeInRecordId == null)
            {
                await context.RespondAsync<ITimeInOutResponse>(new
                {
                    Success = false,
                    Message = "Employee is already timed out"
                });
                return;
            }

            bool success = await _repo.SetTimeOut((int)emp.CurrentTimeInRecordId, DateTimeOffset.Now);
            if (!success)
            {
                await context.RespondAsync<ITimeInOutResponse>(new
                {
                    Success = false,
                    Message = "Employee is already timed out"
                });
                return;
            }

            await context.RespondAsync<ITimeInOutResponse>(new {
                Success = true,
                Message = "Time out success!"
            });
        }
    }
}
