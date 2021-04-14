using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                employee = _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }


        public Compensation CreateCompensation(Compensation compensation)
        {
            if (compensation != null)
            {
                compensation = _employeeRepository.Add(compensation);
                _employeeRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        public List<Compensation> GetCompensationsByEmployeeId(string employeeId)
        {
            if (!String.IsNullOrEmpty(employeeId))
            {
                return _employeeRepository.GetCompensationsByEmployeeId(employeeId);
            }

            return null;
        }

        public ReportingStructure GetReportingByEmployee(Employee employee)
        {
            ReportingStructure ret = null;
            ret = new ReportingStructure()
            {
                Employee = employee,
                NumberOfDirectReports = GetNumberOfDirectReports(employee),
            };
            return ret;
        }

        private int GetNumberOfDirectReports(Employee employee)
        {
            int ret = 0;
            if (employee?.DirectReports != null)
            {
                foreach (var directReport in employee.DirectReports)
                {
                    //Resolve the employee and get the up-to-date count of direct reportees
                    Employee foundDirectReportEmployee = GetById(directReport.EmployeeId);
                    ret += GetNumberOfDirectReports(foundDirectReportEmployee) + 1;
                }
            }
            return ret;
        }
    }
}
