using challenge.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Data
{
    public class EmployeeDataSeeder
    {
        private EmployeeContext _employeeContext;
        private const String EMPLOYEE_SEED_DATA_FILE = "resources/EmployeeSeedData.json";

        public EmployeeDataSeeder(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        public async Task Seed()
        {
            if(!_employeeContext.Employees.Any())
            {
                List<Employee> employees = LoadEmployees();
                _employeeContext.Employees.AddRange(employees);

                await _employeeContext.SaveChangesAsync();
            }
        }

        private List<Employee> LoadEmployees()
        {
            using (FileStream fs = new FileStream(EMPLOYEE_SEED_DATA_FILE, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                List<Employee> employees = serializer.Deserialize<List<Employee>>(jr);
                //FixUpReferences(employees);

                return employees;
            }
        }

        //KDA Removed 4/13/21 - To avoid having to sync up data references, it is best
        //to keep the structure as indicated in the defined JSON schema as a list of employeeIds.
        //For example - When replacement occurs, any reference to the employee would also have to
        //be updated.
        //private void FixUpReferences(List<Employee> employees)
        //{
        //    var employeeIdRefMap = from employee in employees
        //                           select new { Id = employee.EmployeeId, EmployeeRef = employee };

        //    employees.ForEach(employee =>
        //    {

        //        if (employee.DirectReports != null)
        //        {
        //            var referencedEmployees = new List<Employee>(employee.DirectReports.Count);
        //            employee.DirectReports.ForEach(report =>
        //            {
        //                var referencedEmployee = employeeIdRefMap.First(e => e.Id == report.EmployeeId).EmployeeRef;
        //                referencedEmployees.Add(referencedEmployee);
        //            });
        //            employee.DirectReports = referencedEmployees;
        //        }
        //    });
        //}
    }
}
