using challenge.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Compensation Add(Compensation compensation);
        List<Compensation> GetCompensationsByEmployeeId(string employeeId);
        Task SaveAsync();
    }
}