using challenge.Controllers;
using challenge.Data;
using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;
using System.Collections.Generic;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
            Assert.AreEqual(2, employee.DirectReports?.Count ?? 0);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetReporting_Returns_NotFound()
        {
            string invalidEmployeeId = "Invalid_Id";

            // Execute
            var postRequestTask = _httpClient.GetAsync($"api/employee/{invalidEmployeeId}/reporting");
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetReporting_Returns_Ok()
        {
            string johnLennonEmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{johnLennonEmployeeId}/reporting");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reporting = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual("John", reporting.Employee.FirstName);
            Assert.AreEqual("Lennon", reporting.Employee.LastName);
            Assert.AreEqual("Development Manager", reporting.Employee.Position);
            Assert.AreEqual("Engineering", reporting.Employee.Department);

            Assert.AreEqual(4, reporting.NumberOfDirectReports);
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            string johnLennonEmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            DateTime effectiveDateTime = new DateTime(2021, 5, 17);
            int salary = 100;
            var compensation = new Compensation()
            {
                EffectiveDate = effectiveDateTime,
                Salary = salary,
            };

            var newCompensation = PostValidCompensation(johnLennonEmployeeId, compensation);
            Assert.IsNotNull(newCompensation?.CompensationId);
            Assert.AreEqual(newCompensation.EffectiveDate, effectiveDateTime);
            Assert.AreEqual(newCompensation.Employee.EmployeeId, johnLennonEmployeeId);
            Assert.AreEqual(newCompensation.Salary, salary);

            Assert.AreEqual("John", newCompensation.Employee.FirstName);
            Assert.AreEqual("Lennon", newCompensation.Employee.LastName);
            Assert.AreEqual("Development Manager", newCompensation.Employee.Position);
            Assert.AreEqual("Engineering", newCompensation.Employee.Department);
        }

        private Compensation PostValidCompensation(string employeeId, Compensation compensation)
        {
            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync($"api/employee/{employeeId}/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var createdCompensation = response.DeserializeContent<Compensation>();

            return createdCompensation;
        }

        [TestMethod]
        public void CreateCompensation_Multiple_Creation()
        {
            string johnLennonEmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            DateTime firstEffectiveDateTime = new DateTime(2021, 5, 17);
            int firstSalary = 100;
            var firstCompensation = new Compensation()
            {
                EffectiveDate = firstEffectiveDateTime,
                Salary = firstSalary,
            };
            PostValidCompensation(johnLennonEmployeeId, firstCompensation);

            DateTime secondEffectiveDateTime = new DateTime(2021, 6, 17);
            int secondSalary = 200;
            var secondCompensation = new Compensation()
            {
                EffectiveDate = secondEffectiveDateTime,
                Salary = secondSalary,
            };
            PostValidCompensation(johnLennonEmployeeId, secondCompensation);

            var getRequestTask = _httpClient.GetAsync($"api/employee/{johnLennonEmployeeId}/compensation");
            var response = getRequestTask.Result;
            var createdCompensations = response.DeserializeContent<List<Compensation>>();

            Assert.AreEqual(createdCompensations.Count, 2);
            Assert.AreEqual(createdCompensations[0].EffectiveDate, firstEffectiveDateTime);
            Assert.AreEqual(createdCompensations[0].Salary, firstSalary);
            Assert.AreEqual(createdCompensations[1].EffectiveDate, secondEffectiveDateTime);
            Assert.AreEqual(createdCompensations[1].Salary, secondSalary);
        }

        [TestMethod]
        public void CreateCompensation_Returns_NotFound()
        {
            var invalidEmployeeId = "Invalid_Id";
            var compensation = new Compensation()
            {
                EffectiveDate = new DateTime(),
                Salary = 500,
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync($"api/employee/{invalidEmployeeId}/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
