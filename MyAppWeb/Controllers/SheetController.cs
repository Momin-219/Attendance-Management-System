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
            var sheets = _sheetRepository.GetSheets();

            //foreach (var sheet in sheets)
            //{
            //    Console.WriteLine($"EmpID: {sheet.EmpID}, Name: {sheet.Name}, Time: {sheet.Time}, WorkCode: {sheet.WorkCode}, AttendanceState: {sheet.AttendanceState}, DeviceName: {sheet.DeviceName}");
            //}

            return View(sheets); // Returns Views/Sheet/Index.cshtml

            //return View(model);
        }


        public IActionResult Details()
        {
            var sheets = _sheetRepository.GetSheets();
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
