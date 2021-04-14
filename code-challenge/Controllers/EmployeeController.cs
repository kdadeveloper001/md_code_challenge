using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;

namespace challenge.Controllers
{
    [Route("api/employee")]
    public class EmployeeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Received employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        [HttpGet("{id}/reporting")]
        public IActionResult GetReportingByEmployeeId(String id)
        {
            _logger.LogDebug($"Received employee reporting get request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();
            var reporting = _employeeService.GetReportingByEmployee(existingEmployee);
            return Ok(reporting);
        }

        [HttpPost("{employeeId}/compensation")]
        public IActionResult CreateCompensation(String employeeId, [FromBody] Compensation compensation)
        {
            //Should not be able to create a compensation for a non-existant employee
            var employee = _employeeService.GetById(employeeId);
            if (employee == null)
                return NotFound();
            //We'd normally add some validation at a higher middleware level
            //and provide a proper error structure.  Let's make
            //sure we at least don't charge the employee instead of paying them :D
            if (compensation.Salary < 0)
                return BadRequest();
            compensation.Employee = employee;
            _logger.LogDebug($"Received compensation create request for employee '{compensation?.Employee?.EmployeeId ?? ""}'");
            var createdComp = _employeeService.CreateCompensation(compensation);
            if (createdComp == null)
                return BadRequest();
            return CreatedAtRoute("getCompensationByEmployeeId", createdComp);
        }

        [HttpGet("{employeeId}/compensation", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String employeeId)
        {
            _logger.LogDebug($"Received compensation get request for '{employeeId}'");

            var compensations = _employeeService.GetCompensationsByEmployeeId(employeeId);

            if (compensations == null)
                return NotFound();

            return Ok(compensations);
        }
    }
}
