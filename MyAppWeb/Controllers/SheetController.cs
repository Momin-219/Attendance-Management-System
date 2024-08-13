//namespace MyAppWeb.Controllers
//{
//    public class SheetController
//    {
//    }
//}

using Microsoft.AspNetCore.Mvc;
using MyAppWeb.Data;
using MyAppWeb.ViewModels;

namespace MyAppWeb.Controllers
{
    public class SheetController : Controller
    {
        private readonly SheetRepository _sheetRepository;

        public SheetController(SheetRepository sheetRepository)
        {
            _sheetRepository = sheetRepository;
        }

        // GET: /Sheet/Index
        public IActionResult Index()
        {
            //var model = new IndexViewModel
            //{
            //    Sheets = _sheetRepository.GetSheets().ToList()
            //};
            var userName = HttpContext.Session.GetString("UserName");
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            List<SheetViewModel> activitysheet = new List<SheetViewModel>();
            if (userRole=="Admin" || userRole == "HR")
            {
                activitysheet= _sheetRepository.GetSheets();
            }
            else
            {
                activitysheet = _sheetRepository.GetAttendanceRecordsByEmpId(Convert.ToInt32(userId));
            }
           
            //foreach (var sheet in sheets)
            //{
            //    Console.WriteLine($"EmpID: {sheet.EmpID}, Name: {sheet.Name}, Time: {sheet.Time}, WorkCode: {sheet.WorkCode}, AttendanceState: {sheet.AttendanceState}, DeviceName: {sheet.DeviceName}");
            //}

            return View(activitysheet); // Returns Views/Sheet/Index.cshtml

            //return View(model);
        }


        public IActionResult Details()
        {

            var userName = HttpContext.Session.GetString("UserName");
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");



            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login");
            }

            Console.WriteLine("Details Action running in the Sheet Controller ");
            // Debug or log the values
            //System.Diagnostics.Debug.WriteLine($"UserName: {userName}, UserId: {userId}, UserRole: {userRole}");


            ViewBag.UserId = userId;
            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;

            


            var sheets = _sheetRepository.GetSheets();

            if(userRole=="Admin" || userRole == "HR")
            {
                sheets = _sheetRepository.GetSheets();
            }
            else
            {
                sheets = _sheetRepository.GetAttendanceRecordsByEmpId(userId.Value);
            }



            var hoursWorked = _sheetRepository.CalculateHoursWorked(sheets);

            // Prepare data for the view
            var details = new List<DetailsViewModel>();

            foreach (var empHours in hoursWorked)
            {
                int empId = empHours.Key;
                foreach (var dateHours in empHours.Value)
                {
                    string date = dateHours.Key;
                    TimeSpan totalHours = dateHours.Value;

                    // Add record to the details list
                    details.Add(new DetailsViewModel
                    {
                        EmpID = empId,
                        Name = sheets.First(s => s.EmpID == empId).Name,
                        Date = date,
                        HoursWorked = totalHours
                    });
                }
            }

            return View(details);
        }


        //public IActionResult Details()
        //{
        //    var sheets = _sheetRepository.GetSheets();
        //    var hoursSpent = _sheetRepository.CalculateHoursSpent(sheets);

        //    // Prepare data for the view
        //    var details = new List<DetailsViewModel>();

        //    foreach (var empHours in hoursSpent)
        //    {
        //        int empId = empHours.Key;
        //        foreach (var dateHours in empHours.Value)
        //        {
        //            string date = dateHours.Key;
        //            TimeSpan totalHours = dateHours.Value;

        //            // Add record to the details list
        //            details.Add(new DetailsViewModel
        //            {
        //                EmpID = empId,
        //                Name = sheets.First(s => s.EmpID == empId).Name,
        //                Date = date,
        //                HoursWorked = totalHours
        //            });
        //        }
        //    }

        //    return View(details);
        //}

        // Other actions like Create, Edit, Delete can be added here

        public IActionResult Index1()
        {
            //var model = new IndexViewModel
            //{
            //    Sheets = _sheetRepository.GetSheets().ToList()
            //};
            var sheets = _sheetRepository.GetSheets();

            //foreach (var sheet in sheets)
            //{
            //    Console.WriteLine($"EmpID: {sheet.EmpID}, Name: {sheet.Name}, Time: {sheet.Time}, WorkCode: {sheet.WorkCode}, AttendanceState: {sheet.AttendanceState}, DeviceName: {sheet.DeviceName}");
            //}

            return View(sheets); // Returns Views/Sheet/Index.cshtml

            //return View(model);
        }
       
    }
}
