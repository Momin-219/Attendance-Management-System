//namespace MyAppWeb.Data
//{
//    public class EmployeeRepository
//    {
//    }
//}

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyAppWeb.Models;

namespace MyAppWeb.Data
{
    public class EmployeeRepository : ControllerBase
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        [HttpGet]
        public IEnumerable<Employee> GetEmployees()
        
        {
            var employees = new List<Employee>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT EmpID, Name, Role FROM Employees", connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            EmpID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Role = reader.GetString(2)
                        });
                    }
                }
            }

            return employees;
        }

        public Employee GetEmployeeByCredentials(string empId, string name)
        {
            Employee employee = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT EmpID, Name, Role FROM Employees WHERE EmpID = @EmpID AND Name = @Name", connection);
                command.Parameters.AddWithValue("@EmpID", empId);
                command.Parameters.AddWithValue("@Name", name);
                //command.Parameters.AddWithValue("@Role", role);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            EmpID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Role = reader.GetString(2)
                        };
                    }
                }
            }

            return employee;
        }

        public Employee GetUserByUsername(string username)
        {
            Employee employee = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT EmpID, Name, Role FROM Employees WHERE Name = @Name", connection);
                command.Parameters.AddWithValue("@Name", username);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            EmpID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Role = reader.GetString(2)
                        };
                    }
                }
            }

            return employee;
        }

    }
}


