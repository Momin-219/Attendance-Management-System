//namespace MyAppWeb.Controllers
//{
//    public class AccountController
//    {
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MyAppWeb.Data;
using MyAppWeb.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;


namespace MyAppWeb.Controllers
{
    public class AccountController : Controller
    {

        private readonly EmployeeRepository _employeeRepository;
        private readonly SheetRepository _sheetRepository;

        public AccountController(EmployeeRepository employeeRepository, SheetRepository sheetRepository)
        {
            _employeeRepository = employeeRepository;
            _sheetRepository = sheetRepository;
        }


        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public IActionResult Login(string empId, string name, string role)
        {
            // Check credentials against the repository
            var employee = _employeeRepository.GetEmployeeByCredentials(empId, name);

            if (employee != null)
            {
                // Store user information in session
                HttpContext.Session.SetInt32("UserId", employee.EmpID);
                HttpContext.Session.SetString("UserName", employee.Name);
                HttpContext.Session.SetString("UserRole", employee.Role);

                TempData["ShowModal"] = true;
                // Authentication successful
                return RedirectToAction("LoginSuccess");
            }

            // If login fails, return the view with an error message
            //ModelState.AddModelError("", "Invalid login attempt.");
            //return View();

            ViewData["ErrorMessage"] = "Invalid login attempt.";
            return RedirectToAction("LoginFailed");
        }

        [HttpGet]
        public IActionResult LoginSuccess()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");


            //if (userName != null)
            //{
            //    // Assuming you have a method to get the user by their username
            //    var user = GetUserByUsername(userName);
            //    if (user != null)
            //    {
            //        userRole = user.Role; // Set the role from the database
            //    }
            //}

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login");
            }

            //userRole = user.Role;
            ViewBag.UserId=userId;
            ViewBag.UserName = userName;        // Passing username to the view
            ViewBag.UserRole = userRole;        // Passing user role to the view

            // Fetch attendance records for the logged-in user
            var attendanceRecords = _sheetRepository.GetAttendanceRecordsByEmpId(userId.Value);


            // Get the current counter value from session, default to 0 if not set
            var counter = HttpContext.Session.GetInt32("Counter") ?? 0;

            // Increment the counter
            counter++;

            // Save the counter back to session
            HttpContext.Session.SetInt32("Counter", counter);

            // Pass the counter to the view
            ViewBag.Counter = counter;



            return View(attendanceRecords);
        }

        [HttpGet]
        public IActionResult LoginFailed()
        {
            // Clear the session
            HttpContext.Session.Clear();
            return View();
        }

        // Action to handle user logout
        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await HttpContext.SignOutAsync();

            // Redirect to the login page
            return RedirectToAction("Login", "Account");
        }
    }
}
