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
    public class RequestTimeInConsumer : IConsumer<IRequestTimeIn>
    {
        ITimeLogRepository _repo;

        public RequestTimeInConsumer(ITimeLogRepository repo)
        {
            _repo = repo;
        }

        public async Task Consume(ConsumeContext<IRequestTimeIn> context)
        {
            var msg = context.Message;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Employee emp = await _repo.GetEmployeeById(msg.EmployeeId);
            if (emp == null)
            {
                await context.RespondAsync<ITimeInOutResponse>(new
                {
                    Success = false,
                    Message = "Employee does not exist"
                });
                return;
            }

            if (emp.CurrentTimeInRecordId != null)
            {
                await context.RespondAsync<ITimeInOutResponse>(new
                {
                    Success = false,
                    Message = "Employee is already timed in"
                });
                return;
            }

            int id = await _repo.AddTimeIn((int)emp.CurrentTimeInRecordId, DateTimeOffset.Now);
            if (id == 0)
            {
                await context.RespondAsync<ITimeInOutResponse>(new
                {
                    Success = false,
                    Message = "Employee is already timed in"
                });
                return;
            }

            await context.RespondAsync<ITimeInOutResponse>(new
            {
                Success = true,
                Message = "Time in success!"
            });
        }
    }
}
