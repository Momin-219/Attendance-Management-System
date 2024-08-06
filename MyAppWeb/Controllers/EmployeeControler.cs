
using Microsoft.AspNetCore.Mvc;
using MyAppWeb.Data;
using MyAppWeb.Models;

namespace MyAppWeb.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _repository;
        private readonly SheetRepository _repository1;


        public EmployeeController(EmployeeRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var employees = _repository.GetEmployees();

            List<Employee> data=new List<Employee>();
            foreach (var item in employees)
            {
                data.Add(item);
            }

            return View(data);                 // Returns Views/Employee/Index.cshtml
        }
    }
}

