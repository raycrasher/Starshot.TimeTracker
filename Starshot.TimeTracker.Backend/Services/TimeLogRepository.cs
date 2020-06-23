using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Starshot.TimeTracker.Models;

namespace Starshot.TimeTracker.Backend.Services
{
    public interface ITimeLogRepository
    {
        Task<int> AddTimeIn(int employeeId, DateTimeOffset time);
        Task<bool> SetTimeOut(int timeInId, DateTimeOffset time);
        Task<IEnumerable<TimeRecord>> GetCurrentTimeLogsForEachEmployee(int offset, int pageSize);
        Task<int> AddEmployee(Employee employee);
        Task<bool> EditEmployee(Employee employee);
        Task<Employee> GetEmployeeById(int employeeId);
    }

    // Why did I use Dapper/Raw SQL instead of EF Core? 
    // I find that the code is very performant if I can fine tune the actual queries so that I can use the proper indices, just get certain columns, etc.
    // ORMs are OK, but I find this style more comfortable and less error prone nowadays.
    // Testable too - it's very easy to spin up a db container during automated unit tests.
    // Take with a grain of salt.

    public class TimeLogRepository : ITimeLogRepository
    {
        private IDbConnection _connection;

        public TimeLogRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<TimeRecord>> GetCurrentTimeLogsForEachEmployee(int page, int pageSize)
        {
            return await _connection.QueryAsync<TimeRecord>(@"
SELECT 
    TimeRecords.Id, 
    TimeRecords.EmployeeId, 
    TimeRecords.TimeIn, 
    TimeRecords.TimeOut, 
    Employees.EmployeeLastName, 
    Employees.EmployeeFirstName, 
    (Employees.CurrentTimeInRecordId = Id AND TimeOut is NULL) AS IsTimedIn 
FROM TimeRecords
INNER JOIN Employees ON TimeRecords.EmployeeId = Employees.Id
LIMIT @pageSize OFFSET {offset};", 
            new { pageSize,  offset = page * pageSize });
        }


        public async Task<int> AddTimeIn(int employeeId, DateTimeOffset time)
        {
            return await _connection.ExecuteScalarAsync<int>(@"
INSERT INTO TimeRecords ( EmployeeId, TimeIn )
VALUES ( @employeeId, @time );

SET @timeId = last_insert_rowid();

UPDATE Employees SET CurrentTimeInRecordId=@timeId WHERE Id=@employeeId;

SELECT @timeId;", new { employeeId, time });
        }

        public async Task<bool> SetTimeOut(int timeInId, DateTimeOffset time)
        {
            return 0 < await _connection.ExecuteAsync(@"
UPDATE TimeRecords SET TimeOut=@time WHERE Id=@timeInId AND TimeOut is NULL;
UPDATE Employees SET CurrentTimeInRecordId=null WHERE CurrentTimeInRecordId=@timeInId;");
        }


        public async Task<int> AddEmployee(Employee employee)
        {
            employee.Id = await _connection.ExecuteAsync(@"
INSERT INTO Employees ( LastName, FirstName, MiddleName )
VALUES (@LastName, @FirstName, @MiddleName);", employee);
            return employee.Id;
        }

        public async Task<bool> EditEmployee(Employee employee)
        {
            return 0 < await _connection.ExecuteAsync(@"UPDATE Employees SET LastName=@LastName, FirstName=@FirstName, MiddleName=@MiddleName, IsActive=@IsActive WHERE Id=@Id;", employee);
        }

        public Task<Employee> GetEmployeeById(int employeeId)
        {
            return _connection.QuerySingleOrDefaultAsync<Employee>("SELECT * FROM Employees WHERE Id=@employeeId LIMIT 1;", new { employeeId });
        }
    }
}
