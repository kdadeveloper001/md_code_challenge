using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            var ret = _employeeContext.Employees.Add(employee).Entity;
            return ret;
        }

        public Employee GetById(string id)
        {
            //KDA - If we wanted to use the original structure of Employee for direct reports,
            //we could define a max depth N and include DirectReports.DirectReports.....  This may get costly
            //as pulling the top-most employee would grab the entire employee repository and each request for
            //employee information would grab all subordinates.
            return _employeeContext.Employees.Include(nameof(Employee.DirectReports)).SingleOrDefault(e => e.EmployeeId == id);
        }

        public Compensation Add(Compensation compensation)
        {
            return _employeeContext.Compensations.Add(compensation).Entity;
        }

        public List<Compensation> GetCompensationsByEmployeeId(string employeeId)
        {
            return _employeeContext.Compensations.Include(nameof(Employee)).Where(e => e.Employee.EmployeeId == employeeId).ToList();
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
